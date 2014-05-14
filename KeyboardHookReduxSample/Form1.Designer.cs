namespace KeyboardHookReduxSample
{
	partial class Form1
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
			this.buttonStart = new System.Windows.Forms.Button();
			this.textBoxMessages = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonStop = new System.Windows.Forms.Button();
			this.buttonLockdown = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new System.Drawing.Point(73, 220);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(101, 23);
			this.buttonStart.TabIndex = 0;
			this.buttonStart.Text = "Start Hooking";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// textBoxMessages
			// 
			this.textBoxMessages.BackColor = System.Drawing.Color.White;
			this.textBoxMessages.Location = new System.Drawing.Point(12, 42);
			this.textBoxMessages.Multiline = true;
			this.textBoxMessages.Name = "textBoxMessages";
			this.textBoxMessages.ReadOnly = true;
			this.textBoxMessages.Size = new System.Drawing.Size(331, 172);
			this.textBoxMessages.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Hook event messages:";
			// 
			// buttonStop
			// 
			this.buttonStop.Enabled = false;
			this.buttonStop.Location = new System.Drawing.Point(180, 220);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(101, 23);
			this.buttonStop.TabIndex = 3;
			this.buttonStop.Text = "Stop";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// buttonLockdown
			// 
			this.buttonLockdown.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonLockdown.Location = new System.Drawing.Point(226, 12);
			this.buttonLockdown.Name = "buttonLockdown";
			this.buttonLockdown.Size = new System.Drawing.Size(117, 23);
			this.buttonLockdown.TabIndex = 4;
			this.buttonLockdown.Text = "Engage Lockdown";
			this.buttonLockdown.UseVisualStyleBackColor = true;
			this.buttonLockdown.Click += new System.EventHandler(this.buttonLockdown_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 252);
			this.Controls.Add(this.buttonLockdown);
			this.Controls.Add(this.buttonStop);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxMessages);
			this.Controls.Add(this.buttonStart);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.TextBox textBoxMessages;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.Button buttonLockdown;
	}
}

