using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace KeyboardHookReduxSample
{
	static class NativeMethods
	{
		internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern short GetKeyState(int nVirtKey);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

		internal static class KeyStateConstants
		{
			internal const int KEY_PRESSED = 0x8000;
			internal const int WM_KEYDOWN = 0x0100;
			internal const int WM_KEYUP = 0x0101;
			internal const int WM_SYSKEYDOWN = 0x0104;
			internal const int WM_SYSKEYUP = 0x0105;
			internal const int WH_KEYBOARD_LL = 13;
		}
	}
}
