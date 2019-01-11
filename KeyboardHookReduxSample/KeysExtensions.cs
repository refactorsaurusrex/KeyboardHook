using System;
using System.Globalization;
using System.Text;

namespace KeyboardHookReduxSample
{
	/// <summary>
	/// Provides extension methods for use with KeysEx enumerations.
	/// </summary>
	public static class KeysExtensions
	{
		/// <summary>
		/// Returns a friendly string representation of this KeysEx instance.
		/// </summary>
		/// <param name="keys">The keys to convert to a friendly string.</param>
		public static string ToFriendlyString(this KeysEx keys)
		{
			var friendlyString = new StringBuilder();

			if (IsWinLogoModified(keys))
				friendlyString.Append("WinLogo + ");

			if (IsControlModified(keys))
				friendlyString.Append("Ctrl + ");

			if (IsShiftModified(keys))
				friendlyString.Append("Shift + ");

			if (IsAltModified(keys))
				friendlyString.Append("Alt + ");

			string unmodifiedKey = UnmodifiedKey(keys);

			if (string.IsNullOrEmpty(unmodifiedKey) && friendlyString.Length >= 3)
				friendlyString.Remove(friendlyString.Length - 3, 3);
			else
				friendlyString.Append(unmodifiedKey);

			return friendlyString.ToString();
		}

		/// <summary>
		/// Returns KeyCode portion of this instance. That is, the key stripped of modifiers.
		/// </summary>
		/// <param name="keys">The keys to remove modifier keys from.</param>
		static string UnmodifiedKey(KeysEx keys)
		{
			KeysEx keyCode = keys & KeysEx.KeyCode;

			switch (keyCode)
			{
				case KeysEx.Menu:
				case KeysEx.LMenu:
				case KeysEx.RMenu:
				case KeysEx.ShiftKey:
				case KeysEx.LShiftKey:
				case KeysEx.RShiftKey:
				case KeysEx.ControlKey:
				case KeysEx.LControlKey:
				case KeysEx.RControlKey:
				case KeysEx.LWin:
				case KeysEx.RWin:
					return string.Empty;
			}

			switch (keyCode)
			{
				case KeysEx.D0:
				case KeysEx.D1:
				case KeysEx.D2:
				case KeysEx.D3:
				case KeysEx.D4:
				case KeysEx.D5:
				case KeysEx.D6:
				case KeysEx.D7:
				case KeysEx.D8:
				case KeysEx.D9:
					return keyCode.ToString().Remove(0, 1);
			}

			if (keyCode.ToString().ToUpperInvariant().StartsWith("OEM"))
			{
				const uint convertToChar = 2;
				uint keyLiteral = NativeMethods.MapVirtualKey((uint)keyCode, convertToChar);
				return Convert.ToChar(keyLiteral).ToString(CultureInfo.InvariantCulture);
			}

			return keyCode.ToString();
		}

		/// <summary>
		/// Determines whether the specified keys contains the WinLogo modifier, the LWin key, or the RWin key.
		/// </summary>
		/// <param name="keys">The keys to check for logo key modifiers.</param>
		/// <returns>True if this instance contains a logo key modifier; otherwise, false.</returns>
		static bool IsWinLogoModified(KeysEx keys)
		{
			if ((keys & KeysEx.WinLogo) == KeysEx.WinLogo)
				return true;

			KeysEx keyCode = keys & KeysEx.KeyCode;
			switch (keyCode)
			{
				case KeysEx.LWin:
				case KeysEx.RWin:
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether this enumeration contains the Alt modifier.
		/// </summary>
		/// <param name="keys">The keys to check for the Alt modifier.</param>
		/// <returns>True if this enumeration contains the Alt modifier; otherwise false.</returns>
		static bool IsAltModified(KeysEx keys)
		{
			if ((keys & KeysEx.Alt) == KeysEx.Alt)
				return true;

			KeysEx keyCode = keys & KeysEx.KeyCode;
			switch (keyCode)
			{
				case KeysEx.Menu:
				case KeysEx.LMenu:
				case KeysEx.RMenu:
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether this enumeration contains the Shift modifier.
		/// </summary>
		/// <param name="keys">The keys to check for the Shift modifier.</param>
		/// <returns>True if this enumeration contains the Shift modifier; otherwise false.</returns>
		static bool IsShiftModified(KeysEx keys)
		{
			if ((keys & KeysEx.Shift) == KeysEx.Shift)
				return true;

			KeysEx keyCode = keys & KeysEx.KeyCode;
			switch (keyCode)
			{
				case KeysEx.ShiftKey:
				case KeysEx.LShiftKey:
				case KeysEx.RShiftKey:
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether this enumeration contains the Control modifier.
		/// </summary>
		/// <param name="keys">The keys to check for the Control modifier.</param>
		/// <returns>True if this enumeration contains the Control modifier; otherwise false.</returns>
		static bool IsControlModified(KeysEx keys)
		{
			if ((keys & KeysEx.Control) == KeysEx.Control)
				return true;

			KeysEx keyCode = keys & KeysEx.KeyCode;
			switch (keyCode)
			{
				case KeysEx.ControlKey:
				case KeysEx.LControlKey:
				case KeysEx.RControlKey:
					return true;
			}

			return false;
		}
	}
}
