namespace IPScanner
{
	partial class RangeSelector
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
			this.label1 = new System.Windows.Forms.Label();
			this.lvRanges = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Click an IP range:";
			// 
			// lvRanges
			// 
			this.lvRanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvRanges.Location = new System.Drawing.Point(12, 29);
			this.lvRanges.MultiSelect = false;
			this.lvRanges.Name = "lvRanges";
			this.lvRanges.Size = new System.Drawing.Size(260, 220);
			this.lvRanges.TabIndex = 1;
			this.lvRanges.UseCompatibleStateImageBehavior = false;
			this.lvRanges.View = System.Windows.Forms.View.List;
			this.lvRanges.SelectedIndexChanged += new System.EventHandler(this.lvRanges_SelectedIndexChanged);
			// 
			// RangeSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.lvRanges);
			this.Controls.Add(this.label1);
			this.Name = "RangeSelector";
			this.Text = "RangeSelector";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView lvRanges;
	}
}