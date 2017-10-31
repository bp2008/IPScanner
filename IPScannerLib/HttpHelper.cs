using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Net.Configuration;

namespace IPScanner
{
	public class HttpResponseData
	{
		public string data;
		public SortedList<string, string> headers;
		public string host;
		public HttpResponseData(string data, SortedList<string, string> headers, string host)
		{
			this.data = data;
			this.headers = headers;
			this.host = host;
		}
		public string GetHeaderValue(string key)
		{
			string val;
			if (headers.TryGetValue(key.ToLower(), out val))
				return val;
			return "";
		}
	}
	internal static class HttpHelper
	{
		static HttpHelper()
		{
			ToggleAllowUnsafeHeaderParsing(true);
		}
		public static HttpResponseData GetHttpResponseData(string url)
		{
			SortedList<string, string> headers = new SortedList<string, string>();
			//return new HttpResponseData("", headers, url);
			byte[] data = GetData(url, headers);
			return new HttpResponseData(UTF8Encoding.UTF8.GetString(data), headers, url);
		}
		/// <summary>
		/// Gets data from a URL and returns it as a byte array.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static byte[] GetData(string url, SortedList<string, string> headers = null, string user = "", string password = "", bool keepAlive = false)
		{
			try
			{
				if (url.Contains(".80"))
				{
					Console.WriteLine(url);
				}
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
				webRequest.Proxy = null;
				webRequest.KeepAlive = keepAlive;
				webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

				if (!string.IsNullOrEmpty(user) || !string.IsNullOrEmpty(password))
				{
					string authInfo = user + ":" + password;
					authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
					webRequest.Headers["Authorization"] = "Basic " + authInfo;
				}
				webRequest.Method = "GET";
				webRequest.Timeout = 5000;
				webRequest.AllowAutoRedirect = true;
				return GetResponse(webRequest, headers);
			}
			catch (ThreadAbortException ex) { throw ex; }
			catch (WebException ex)
			{
				if (ex.Message.StartsWith("The server committed a protocol violation"))
					return UTF8Encoding.UTF8.GetBytes(ex.Message);
				if (ex.Message == "The remote server returned an error: (404) Not Found." || ex.Message == "The remote server returned an error: (401) Unauthorized.")
				{

					//if(ex.Response.ResponseUri.AbsolutePath == "/nocookies.html")
					try
					{
						return GetResponseData(ex.Response, headers);
					}
					catch (ThreadAbortException e) { throw e; }
					catch (Exception e)
					{
						if (url.Contains(".80"))
						{
							Console.WriteLine(e.ToString());
						}
					}
				}
				//else if (ex.Message == "The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel." && url.StartsWith("http:"))
				//{
				//    url = "https" + url.Substring(4);
				//    return GetData(url, headers, user, password, keepAlive);
				//}
			}
			catch (Exception ex)
			{
				if (url.Contains(".80"))
				{
					Console.WriteLine(ex.ToString());
				}
			}
			return new byte[0];
		}
		private static byte[] GetResponse(HttpWebRequest webRequest, SortedList<string, string> headers = null)
		{
			return GetResponseData((HttpWebResponse)webRequest.GetResponse(), headers);
		}

		private static byte[] GetResponseData(WebResponse webResponseObj, SortedList<string, string> headers = null)
		{
			byte[] data;
			using (HttpWebResponse webResponse = (HttpWebResponse)webResponseObj)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (Stream responseStream = webResponse.GetResponseStream())
					{
						// Dump the response stream into the MemoryStream ms
						int bytesRead = 1;
						while (bytesRead > 0)
						{
							byte[] buffer = new byte[8000];
							bytesRead = responseStream.Read(buffer, 0, buffer.Length);
							if (bytesRead > 0)
								ms.Write(buffer, 0, bytesRead);
						}
						data = new byte[ms.Length];

						// Dump the data into the byte array
						ms.Seek(0, SeekOrigin.Begin);
						ms.Read(data, 0, data.Length);
						responseStream.Close();

						if (headers != null)
							foreach (string key in webResponse.Headers.AllKeys)
								headers[key.ToLower()] = webResponse.Headers[key];
					}
				}
				webResponse.Close();
			}
			return data;
		}

		/// <summary>
		/// Enable/disable useUnsafeHeaderParsing.
		/// See http://o2platform.wordpress.com/2010/10/20/dealing-with-the-server-committed-a-protocol-violation-sectionresponsestatusline/
		/// </summary>
		/// <param name="enable"></param>
		/// <returns></returns>
		public static bool ToggleAllowUnsafeHeaderParsing(bool enable)
		{
			//Get the assembly that contains the internal class
			Assembly assembly = Assembly.GetAssembly(typeof(SettingsSection));
			if (assembly != null)
			{
				//Use the assembly in order to get the internal type for the internal class
				Type settingsSectionType = assembly.GetType("System.Net.Configuration.SettingsSectionInternal");
				if (settingsSectionType != null)
				{
					//Use the internal static property to get an instance of the internal settings class.
					//If the static instance isn't created already invoking the property will create it for us.
					object anInstance = settingsSectionType.InvokeMember("Section", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
					if (anInstance != null)
					{
						//Locate the private bool field that tells the framework if unsafe header parsing is allowed
						FieldInfo aUseUnsafeHeaderParsing = settingsSectionType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
						if (aUseUnsafeHeaderParsing != null)
						{
							aUseUnsafeHeaderParsing.SetValue(anInstance, enable);
							return true;
						}

					}
				}
			}
			return false;
		}
	}
}
