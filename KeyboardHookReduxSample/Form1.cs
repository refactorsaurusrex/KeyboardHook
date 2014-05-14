using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyboardHookReduxSample
{
	public partial class Form1 : Form
	{
		readonly KeyboardHook hook = new KeyboardHook();

		public Form1()
		{
			InitializeComponent();

			KeyboardHook.LockedDownKeyboardKeyPressed += KeyboardHook_LockedDownKeyboardKeyPressed;

			hook.AutoRepeat = true;
			hook.SetKeys(new KeyCombination(KeysEx.WinLogo | KeysEx.D));
			hook.Pressed += hook_Pressed;
		}

		void buttonStart_Click(object sender, EventArgs e)
		{
			hook.Engage();
			buttonStart.Enabled = false;
			buttonStop.Enabled = true;
		}

		void hook_Pressed(object sender, EventArgs e)
		{
			// Invoke is required here to avoid cross threading exceptions.
			Action a = () => textBoxMessages.AppendText(string.Format("Hook pressed event: {0}{1}", DateTime.Now, Environment.NewLine));
			textBoxMessages.Invoke(a);
		}

		void buttonStop_Click(object sender, EventArgs e)
		{
			hook.Disengage();

			buttonStart.Enabled = true;
			buttonStop.Enabled = false;
		}

		bool isLockedDown;
		void buttonLockdown_Click(object sender, EventArgs e)
		{
			if (isLockedDown)
			{
				KeyboardHook.ReleaseFullKeyboardLockdown();
				buttonLockdown.Text = "Engage Lockdown";
			}
			else
			{
				KeyboardHook.EngageFullKeyboardLockdown();
				buttonLockdown.Text = "Release Lockdown";
			}

			isLockedDown = !isLockedDown;
		}

		void KeyboardHook_LockedDownKeyboardKeyPressed(object sender, KeyboardLockDownKeyPressedEventArgs e)
		{
			// Invoke is required here to avoid cross threading exceptions.
			Action a = () => textBoxMessages.AppendText(string.Format("Lockdown pressed event: {0}{1}", DateTime.Now, Environment.NewLine));
			textBoxMessages.Invoke(a);
		}
	}
}
