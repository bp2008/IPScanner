namespace IPScanner
{
	partial class ResponseAnalyze
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtHeaders = new System.Windows.Forms.TextBox();
			this.txtBody = new System.Windows.Forms.TextBox();
			this.btnOpenInBrowser = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtHeaders
			// 
			this.txtHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtHeaders.Location = new System.Drawing.Point(12, 31);
			this.txtHeaders.Multiline = true;
			this.txtHeaders.Name = "txtHeaders";
			this.txtHeaders.Size = new System.Drawing.Size(549, 131);
			this.txtHeaders.TabIndex = 0;
			this.txtHeaders.DoubleClick += new System.EventHandler(this.txtHeaders_DoubleClick);
			// 
			// txtBody
			// 
			this.txtBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtBody.Location = new System.Drawing.Point(12, 168);
			this.txtBody.Multiline = true;
			this.txtBody.Name = "txtBody";
			this.txtBody.Size = new System.Drawing.Size(549, 159);
			this.txtBody.TabIndex = 1;
			this.txtBody.DoubleClick += new System.EventHandler(this.txtBody_DoubleClick);
			// 
			// btnOpenInBrowser
			// 
			this.btnOpenInBrowser.Location = new System.Drawing.Point(390, 2);
			this.btnOpenInBrowser.Name = "btnOpenInBrowser";
			this.btnOpenInBrowser.Size = new System.Drawing.Size(171, 23);
			this.btnOpenInBrowser.TabIndex = 2;
			this.btnOpenInBrowser.Text = "Open in Browser";
			this.btnOpenInBrowser.UseVisualStyleBackColor = true;
			this.btnOpenInBrowser.Click += new System.EventHandler(this.btnOpenInBrowser_Click);
			// 
			// ResponseAnalyze
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(573, 339);
			this.Controls.Add(this.btnOpenInBrowser);
			this.Controls.Add(this.txtBody);
			this.Controls.Add(this.txtHeaders);
			this.Name = "ResponseAnalyze";
			this.Text = "Response Analysis";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtHeaders;
		private System.Windows.Forms.TextBox txtBody;
		private System.Windows.Forms.Button btnOpenInBrowser;
	}
}