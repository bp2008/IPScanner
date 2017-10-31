using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace IPScanner
{
	public partial class IPScannerForm : Form
	{
		NetworkScanner scanner = new NetworkScanner();
		Timer timer = new Timer();
		public IPScannerForm()
		{
			InitializeComponent();
			List<Tuple<IPAddress, IPAddress>> ipRanges = IPRanges.GetOperationalIPRanges();
			if (ipRanges.Count > 0)
			{
				ipFrom.IPAddress = ipRanges[0].Item1;
				ipTo.IPAddress = ipRanges[0].Item2;
			}
		}

		List<IPScanResult> results;
		private void btnScan_Click(object sender, EventArgs e)
		{
			timer.Stop();
			lvIPList.Items.Clear();
			results = scanner.BeginScan(ipFrom.IPAddress, ipTo.IPAddress);
			timer.Interval = 1000;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			PopulateListView();
		}

		private void PopulateListView()
		{
			bool itemModified = false;
			for (int i = 0; i < results.Count; i++)
			{
				IPScanResult result = results[i];
				if (result.status == ScanStatus.Complete || result.status == ScanStatus.Partial)
				{
					string ip = result.ip.ToString();
					ListViewItem[] matchedItems = lvIPList.Items.Find(ip, false);
					if (matchedItems.Length > 0)
					{
						matchedItems[0].Tag = result.response;
						matchedItems[0].SubItems[0].Text = result.ip.ToString();
						matchedItems[0].SubItems[1].Text = GetPingTime(result);
						matchedItems[0].SubItems[2].Text = result.host;
						matchedItems[0].SubItems[3].Text = result.identification;
					}
					else
					{
						ListViewItem lvi = new ListViewItem(new string[] { result.ip.ToString(), GetPingTime(result), result.host, result.identification });
						lvi.Name = ip;
						lvIPList.Items.Add(lvi);
					}
					itemModified = true;
				}
			}
		}

		private string GetPingTime(IPScanResult result)
		{
			if (result.ping > -1)
				return result.ping + " ms";
			return "N/A";
		}

		private void IPScannerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer.Stop();
			scanner.Abort();
		}

		private void btnAnalyzeSelected_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lvIPList.SelectedItems)
			{
				if (item.Tag != null)
				{
					HttpResponseData response = (HttpResponseData)item.Tag;
					ResponseAnalyze ra = new ResponseAnalyze(response);
					ra.Show();
				}
			}
		}

		private void btnRanges_Click(object sender, EventArgs e)
		{
			RangeSelector rs = new RangeSelector();
			rs.ShowDialog();
			if (rs.selectedRange != null)
			{
				ipFrom.IPAddress = rs.selectedRange.Item1;
				ipTo.IPAddress = rs.selectedRange.Item2;
			}
		}
	}
}
