using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Amib.Threading;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace IPScanner
{
	public class NetworkScanner
	{
		private static Regex rxHtmlTitle = new Regex("<title>([^<]+?)</title>", RegexOptions.Compiled);
		SmartThreadPool Pool = new SmartThreadPool(1000, 256, 0);
		public NetworkScanner()
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			System.Net.ServicePointManager.MaxServicePoints = int.MaxValue;
		}

		public List<IPScanResult> BeginScan(IPAddress ipFrom, IPAddress ipTo)
		{
			Amib.Threading.Action<IPAddress, List<IPScanResult>, int> ipScanAction = new Amib.Threading.Action<IPAddress, List<IPScanResult>, int>(ScanIPAsync);
			// Count the IP addresses included in this range
			byte[] addyEnd = ipTo.GetAddressBytes();
			byte[] addyNext = ipFrom.GetAddressBytes();

			List<IPScanResult> Results = new List<IPScanResult>();
			while (CompareIPs(addyNext, addyEnd) < 1)
			{
				Results.Add(new IPScanResult(new IPAddress(addyNext)));
				IncrementIP(addyNext);
			}

			for (int i = 0; i < Results.Count; i++)
				Pool.QueueWorkItem(ipScanAction, Results[i].ip, Results, i);
			return Results;
		}
		private void ScanIPAsync(IPAddress ip, List<IPScanResult> results, int listIndex)
		{
			bool foundHost = false;
			results[listIndex].status = ScanStatus.Initializing;

			// Attempt Ordinary Ping
			try
			{
				using (Ping p = new Ping())
				{
					PingReply pingReply = p.Send(ip, 5000);
					if (pingReply.Status == IPStatus.Success)
					{
						foundHost = true;
						results[listIndex].status = ScanStatus.Partial;
						results[listIndex].ping = (int)pingReply.RoundtripTime;
					}
				}
			}
			catch (SocketException)
			{
			}
			catch (Exception)
			{
			}

			// Attempt DNS Lookup
			try
			{
				Stopwatch timer = new Stopwatch();
				timer.Start();
				IPHostEntry ipe = Dns.GetHostEntry(ip);
				timer.Stop();
				int dnsLookupTime = (int)timer.ElapsedMilliseconds;

				foundHost = true;
				//if (results[listIndex].ping < 0 || dnsLookupTime < results[listIndex].ping)
				//	results[listIndex].ping = dnsLookupTime;
				results[listIndex].host = ipe.HostName.ToString();
				results[listIndex].status = ScanStatus.Complete;
			}
			//catch (SocketException ex)
			//{
			//	//if (ex.SocketErrorCode == SocketError.HostNotFound)
			//	//    return;
			//	Console.WriteLine(ex.Message);
			//}
			catch (Exception)
			{
			}



			if (foundHost)
			{
				// Try to identify
				HttpResponseData response;
				results[listIndex].identification = IdentifyHost(ip, out response);
				results[listIndex].status = ScanStatus.Complete;
				results[listIndex].response = response;
			}
			else
				results[listIndex].status = ScanStatus.NotFound;
		}

		private string IdentifyHost(IPAddress ip, out HttpResponseData response)
		{
			response = null;
			Stopwatch sw = new Stopwatch();
			try
			{
				sw.Start();
				response = HttpHelper.GetHttpResponseData("http://" + ip.ToString() + "/");
				if (response.GetHeaderValue("server").StartsWith("lighttpd") && response.GetHeaderValue("set-cookie").Contains("AIROS_") && response.data.Contains("<title>Error 404"))
					return "Ubiquiti";
				else if (response.GetHeaderValue("server").StartsWith("Boa") && response.data.Contains("<OBJECT ID=\"TSConfigIPCCtrl\""))
					return "Generic IP Cam"; // CCDCam EC-IP5911
				else if (response.data.Contains("flow_slct = get_slctid('flowtype');"))
					return "IPS Cam";
				else if (response.GetHeaderValue("server") == "GoAhead-Webs" && response.data.Contains("document.location = '/live.asp?"))
					return "Edimax Cam";
				else if (response.GetHeaderValue("server").StartsWith("App-webs/") && response.data.Contains("window.location.href = \"doc/page/login.asp"))
					return "Hikvision";
				else if (response.data.Contains("src=\"jsCore/LAB.js\"") || response.data.Contains("var lt = \"?WebVersion=") || response.data.Contains("src=\"jsCore/rpcCore.js"))
					return "Dahua";
				else if (response.GetHeaderValue("www-authenticate").Contains("realm=\"tomato\""))
					return "Tomato";
				else if (response.GetHeaderValue("server") == "Web Server" && response.data.Contains("<TITLE>NETGEAR FS728TP</TITLE>"))
					return "Netgear FS728TP";
				else if (response.GetHeaderValue("set-cookie").Contains("DLILPC=") && response.data.Contains("<title>Power Controller"))
					return "Web Power Switch";
				else if (response.data == "The server committed a protocol violation. Section=ResponseStatusLine")
					return "? WeatherDirect ?";
				else if (response.data == "The server committed a protocol violation. Section=ResponseHeader Detail=CR must be followed by LF")
					return "? Web Power Switch ?";
				else if (response.data.Contains("NetDAQ ND-100"))
					return "NetDAQ ND-100";
				else if (response.GetHeaderValue("server") == "nginx" && response.data.Contains("<title>airVision:"))
					return "AirVision NVR";
				else if (response.GetHeaderValue("server") == "nginx" && response.data.Contains("<title>airVision:"))
					return "AirVision NVR";
				else if (response.GetHeaderValue("server").StartsWith("BlueIris-"))
					return "Blue Iris";
				//else if (response.data.Contains("<title>iTach"))
				//	return "iTach";
				else if (response.data.Contains("href=\"/cmh\""))
					return "Vera";
				else if (response.data.Contains("WDMyCloud"))
					return "WDMyCloud";
				//else if (response.data.Contains("<title>DD-WRT"))
				//	return "DD-WRT";
				else if (response.data.Contains("= \"Peplink\""))
					return "Peplink";
				else if (response.data.Contains("GSViewerX.ocx"))
					return "GrandStream";
				else if (response.data.Contains("content=\"Canon Inc.\""))
					return "Canon printer";
				else if (response.GetHeaderValue("server") == "tsbox" && response.GetHeaderValue("www-authenticate") == "Basic realm=\"pbox\"")
					return "HDMI Encoder";
				else if (response.data.Contains("Rules of login password.\\n"))
					return "ACTi";
				else if (response.data.Contains("/static/freenas_favicon.ico"))
					return "FreeNAS";
				else if (response.data.Contains("CONTENT=\"0;url=cgi-bin/kvm.cgi\""))
					return "Avocent KVM";
				else if (response.GetHeaderValue("www-authenticate") == "Basic realm=\"TomatoUSB\"")
					return "TomatoUSB Router";
				else if (response.GetHeaderValue("auther") == "Steven Wu" && response.GetHeaderValue("server") == "Camera Web Server/1.0" && response.data.Contains("location.href=\"top.htm?Currenttime=\"+timeValue;"))
					return "TrendNET IP cam";
				else if (response.data.Contains(@"<meta http-equiv=""refresh"" content=""0;URL='/ui'""/>"))
					return "ESXi";
				else if (response.GetHeaderValue("server") == "Microsoft-HTTPAPI/2.0")
					return "IIS";
				else
				{
					Match m = rxHtmlTitle.Match(response.data);
					if (m.Success)
						return m.Groups[1].Value;
					string server = response.GetHeaderValue("server");
					if (!string.IsNullOrEmpty(server))
						return server;
					return "";
				}
				return response.data;
			}
			catch (Exception)
			{
			}
			finally
			{
				sw.Stop();
				//Console.WriteLine("Spent " + sw.ElapsedMilliseconds + " on " + response.data.Length);
			}
			return "";
		}

		public void Abort()
		{
			Pool.Cancel(true);
		}
		bool ArraysMatch(Array a1, Array a2)
		{
			if (a1.Length != a2.Length)
				return false;
			for (int i = 0; i < a1.Length; i++)
				if (a1.GetValue(i) != a1.GetValue(i))
					return false;
			return true;
		}
		int CompareIPs(byte[] ip1, byte[] ip2)
		{
			if (ip1 == null || ip1.Length != 4)
				return -1;
			if (ip2 == null || ip2.Length != 4)
				return 1;
			int comp = ip1[0].CompareTo(ip2[0]);
			if (comp == 0)
				comp = ip1[1].CompareTo(ip2[1]);
			if (comp == 0)
				comp = ip1[2].CompareTo(ip2[2]);
			if (comp == 0)
				comp = ip1[3].CompareTo(ip2[3]);
			return comp;
		}
		void IncrementIP(byte[] ip, int idx = 3)
		{
			if (ip == null || ip.Length != 4 || idx < 0)
				return;
			if (ip[idx] == 254)
			{
				ip[idx] = 1;
				IncrementIP(ip, idx - 1);
			}
			else
				ip[idx] = (byte)(ip[idx] + 1);
		}
	}
}
