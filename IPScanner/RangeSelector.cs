using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace IPScanner
{
	public partial class RangeSelector : Form
	{
		public Tuple<IPAddress, IPAddress> selectedRange = null;
		public RangeSelector()
		{
			InitializeComponent();
			foreach (Tuple<IPAddress, IPAddress> range in IPRanges.GetOperationalIPRanges())
			{
				ListViewItem item = new ListViewItem();
				item.Tag = range;
				item.Text = range.Item1.ToString() + " - " + range.Item2.ToString();
				lvRanges.Items.Add(item);
			}
		}

		private void lvRanges_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedRange = (Tuple<IPAddress, IPAddress>)lvRanges.SelectedItems[0].Tag;
			this.Close();
		}
	}
}
