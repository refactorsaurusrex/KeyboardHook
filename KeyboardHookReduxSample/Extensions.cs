using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyboardHookReduxSample
{
	public static class Extensions
	{
		/// <summary>
		/// Converts the specified Keys to a KeysEx enumeration.
		/// </summary>
		/// <param name="keys">The keys to convert.</param>
		public static KeysEx ToKeysEx(this Keys keys)
		{
			return EnumParser.Parse<KeysEx>(keys.ToString());
		}
	}
}
