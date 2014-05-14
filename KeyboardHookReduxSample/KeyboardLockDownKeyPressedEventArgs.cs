using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyboardHookReduxSample
{
	/// <summary>
	/// Provides information regarding KeyboardLockDownKeyPressed events.
	/// </summary>
	public class KeyboardLockDownKeyPressedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyboardLockDownKeyPressedEventArgs"/> class.
		/// </summary>
		/// <param name="keys">The keys that were pressed.</param>
		/// <param name="state">The state of the pressed keys.</param>
		/// <remarks></remarks>
		public KeyboardLockDownKeyPressedEventArgs(KeysEx keys, KeyState state)
		{
			Keys = keys;
			State = state;
		}

		/// <summary>
		/// Gets the keys that were pressed.
		/// </summary>
		public KeysEx Keys { get; private set; }

		/// <summary>
		/// Gets the state of the pressed keys.
		/// </summary>
		public KeyState State { get; private set; }
	}
}
