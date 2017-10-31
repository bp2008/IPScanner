using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace IPScanner
{
	public static class IPRanges
	{
		public static List<Tuple<IPAddress, IPAddress>> GetOperationalIPRanges()
		{
			List<Tuple<IPAddress, IPAddress>> ranges = new List<Tuple<IPAddress, IPAddress>>();

			foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (netInterface.OperationalStatus != OperationalStatus.Up)
					continue;
				IPInterfaceProperties ipProps = netInterface.GetIPProperties();
				foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
				{
					if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
						ranges.Add(new Tuple<IPAddress, IPAddress>(GetLowestInRange(addr.Address, addr.IPv4Mask), GetHighestInRange(addr.Address, addr.IPv4Mask)));
				}
			}
			return ranges;
		}
		private static IPAddress GetLowestInRange(IPAddress address, IPAddress mask)
		{
			byte[] addressBytes = address.GetAddressBytes();
			byte[] maskBytes = mask.GetAddressBytes();
			if (addressBytes.Length != 4 || maskBytes.Length != 4)
				return IPAddress.None;
			byte[] lowest = new byte[4];
			for (var i = 0; i < 4; i++)
				lowest[i] = (byte)(addressBytes[i] & maskBytes[i]);
			return new IPAddress(lowest);
		}
		private static IPAddress GetHighestInRange(IPAddress address, IPAddress mask)
		{
			byte[] addressBytes = address.GetAddressBytes();
			byte[] maskBytes = mask.GetAddressBytes();
			if (addressBytes.Length != 4 || maskBytes.Length != 4)
				return IPAddress.None;
			byte[] highest = new byte[4];
			for (var i = 0; i < 4; i++)
				highest[i] = (byte)((addressBytes[i] & maskBytes[i]) | ~maskBytes[i]);
			return new IPAddress(highest);
		}
	}
}
