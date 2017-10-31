using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace IPScanner
{
	public class IPScanResult
	{
		public IPAddress ip;
		public int ping = -1;
		public string host;
		public ScanStatus status = ScanStatus.Initializing;
		public string identification = "...";
		public HttpResponseData response;

		public IPScanResult(IPAddress ip)
		{
			this.ip = ip;
		}
		//public IPScanResult(IPAddress ip, int ping, string host)
		//{
		//    this.ip = ip;
		//    this.ping = ping;
		//    this.host = host;
		//    this.status = ScanStatus.Complete;
		//}
	}
}
