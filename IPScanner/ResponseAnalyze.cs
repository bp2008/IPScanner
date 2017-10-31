using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPScanner
{
	public partial class ResponseAnalyze : Form
	{
		string url = "";
		public ResponseAnalyze(HttpResponseData response)
		{
			InitializeComponent();
			txtHeaders.Text = string.Join(Environment.NewLine, response.headers.Select(kvp => { return kvp.Key + ": " + kvp.Value; }));
			txtBody.Text = response.data;
			this.Text = "Analysis: " + response.host;
			url = response.host;
		}

		private void txtBody_DoubleClick(object sender, EventArgs e)
		{
			txtBody.SelectAll();
		}

		private void txtHeaders_DoubleClick(object sender, EventArgs e)
		{
			txtHeaders.SelectAll();
		}

		private void btnOpenInBrowser_Click(object sender, EventArgs e)
		{
			Process.Start(url);
		}
	}
}
