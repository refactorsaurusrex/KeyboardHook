using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Permissions;
using System.Linq;

namespace KeyboardHookReduxSample
{
	/// <summary>
	/// Represents a global low level keyboard hook.
	/// </summary>
	public class KeyboardHook : IDisposable
	{
		const int keyUp = NativeMethods.KeyStateConstants.WM_KEYUP;
		const int systemKeyUp = NativeMethods.KeyStateConstants.WM_SYSKEYUP;
		const int keyDown = NativeMethods.KeyStateConstants.WM_KEYDOWN;
		const int systemKeyDown = NativeMethods.KeyStateConstants.WM_SYSKEYDOWN;

		static readonly HashSet<KeyCombination> currentHookKeys = new HashSet<KeyCombination>();
		static readonly NativeMethods.LowLevelKeyboardProc lockdownHookCallBack = LockdownHookCallBack;
		static bool isKeyboardLockedDown;
		static IntPtr lockdownHookId;
		static KeysEx lockedDownKeys;

		IntPtr hookId;
		bool isDisposed;
		bool keyReleased;
		readonly NativeMethods.LowLevelKeyboardProc hookCallback;

		/// <summary>
		/// Occurs when a key is pressed while keyboard lockdown is engaged.
		/// </summary>
		public static event EventHandler<KeyboardLockDownKeyPressedEventArgs> LockedDownKeyboardKeyPressed = delegate { };

		/// <summary>
		/// Engages a full keyboard lockdown, which disables all keyboard processing beyond the LockedDownKeyboardKeyPressed event.
		/// </summary>
		public static void EngageFullKeyboardLockdown()
		{
			lockdownHookId = NativeMethods.SetWindowsHookEx(
				NativeMethods.KeyStateConstants.WH_KEYBOARD_LL,
				lockdownHookCallBack,
				IntPtr.Zero,
				0);

			if (lockdownHookId == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			isKeyboardLockedDown = true;
		}

		/// <summary>
		/// Releases keyboard lockdown and allows keyboard processing as normal.
		/// </summary>
		public static void ReleaseFullKeyboardLockdown()
		{
			if (lockdownHookId != IntPtr.Zero)
			{
				NativeMethods.UnhookWindowsHookEx(lockdownHookId);
				lockdownHookId = IntPtr.Zero;
			}

			isKeyboardLockedDown = false;
		}

		/// <summary>
		/// Determines whether the specified key combination is already in use.
		/// </summary>
		/// <param name="combination">The combination to check.</param>
		/// <returns>True if the key combination is already in use; otherwise, false.</returns>
		public static bool IsKeyCombinationTaken(KeyCombination combination)
		{
			return currentHookKeys.Contains(combination);
		}

		/// <summary>
		/// Raises the KeyboardLockDownKeyPressed event by invoking each subscribed delegate asynchronously.
		/// </summary>
		/// <param name="pressedKeys">The keys that are pressed.</param>
		/// <param name="state">The state of the keys.</param>
		static void OnLockedDownKeyboardKeyPressed(KeysEx pressedKeys, KeyState state)
		{
			foreach (EventHandler<KeyboardLockDownKeyPressedEventArgs> pressedEvent in LockedDownKeyboardKeyPressed.GetInvocationList())
			{
				var args = new KeyboardLockDownKeyPressedEventArgs(pressedKeys, state);
				AsyncCallback callback = ar => ((EventHandler<KeyboardLockDownKeyPressedEventArgs>)ar.AsyncState).EndInvoke(ar);
				pressedEvent.BeginInvoke(null, args, callback, pressedEvent);
			}
		}

		/// <summary>
		/// Returns a value indicating whether a non-modifier key is currently pressed.
		/// </summary>
		/// <param name="key">The key to check. Key must not be a modifier bit mask.</param>
		/// <returns>True if the key is currently pressed; otherwise false.</returns>
		/// <exception cref="ArgumentException">Occurs if key is a modifier bit mask. </exception>
		static bool IsKeyPressed(KeysEx key)
		{
			if ((key & KeysEx.Modifiers) == KeysEx.Modifiers)
			{
				throw new ArgumentException("Key cannot contain any modifiers.", "key");
			}

			const int keyPressed = NativeMethods.KeyStateConstants.KEY_PRESSED;
			return (NativeMethods.GetKeyState((int)key) & keyPressed) == keyPressed;
		}

		/// <summary>
		/// Callback handler for keyboard lockdowns.
		/// </summary>
		static IntPtr LockdownHookCallBack(int nCode, IntPtr wParam, IntPtr lParam)
		{
			int keyStateParam = (int)wParam;
			KeysEx pressedKey = (KeysEx)Marshal.ReadInt32(lParam);

			switch (pressedKey)
			{
				case KeysEx.LControlKey:
				case KeysEx.RControlKey:
					pressedKey = KeysEx.Control;
					break;

				case KeysEx.RMenu:
				case KeysEx.LMenu:
					pressedKey = KeysEx.Alt;
					break;

				case KeysEx.LShiftKey:
				case KeysEx.RShiftKey:
					pressedKey = KeysEx.Shift;
					break;

				case KeysEx.LWin:
				case KeysEx.RWin:
					pressedKey = KeysEx.WinLogo;
					break;
			}

			KeyState keyState;
			if (keyStateParam == keyUp || keyStateParam == systemKeyUp)
			{
				keyState = KeyState.Up;
				lockedDownKeys ^= pressedKey;
			}
			else if (keyStateParam == keyDown || keyStateParam == systemKeyDown)
			{
				keyState = KeyState.Down;
				lockedDownKeys |= pressedKey;
			}
			else
			{
				throw new ArgumentOutOfRangeException("wParam", "Invalid key state detected.");
			}

			OnLockedDownKeyboardKeyPressed(lockedDownKeys, keyState);
			return new IntPtr(1);
		}

		/// <summary>
		/// Initializes a new instance of the KeyboardHook class.
		/// </summary>
		public KeyboardHook()
		{
			hookCallback = HookCallBack;
			Combination = new KeyCombination(KeysEx.None);
		}

		/// <summary>
		/// Finalizes an instance of the KeyboardHook class.
		/// </summary>
		~KeyboardHook()
		{
			Dispose(false);
		}

		/// <summary>
		/// Occurs when the KeyboardHook's assigned key combination is pressed.
		/// </summary>
		public event EventHandler Pressed = delegate { };

		/// <summary>
		/// Gets the key combination for this hook.
		/// </summary>
		public KeyCombination Combination { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether to allow normal processing of key strokes after 
		/// the hook has finished processing it.
		/// </summary>
		public bool AllowPassThrough { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the KeyboardHook Pressed event is fired repeatedly for the duration
		/// of the key press or only once per key press.
		/// </summary>
		public bool AutoRepeat { get; set; }

		/// <summary>
		/// Gets a value indicating whether the KeyboardHook is currently active.
		/// </summary>
		public bool IsEngaged
		{
			get { return hookId != IntPtr.Zero; }
		}

		/// <summary>
		/// Removes the keys associated with this hook. This can only be performed when the hook is not active.
		/// </summary>
		/// <exception cref="InvalidOperationException">Occurs if the hook is currently engaged.</exception>
		public void RemoveKeys()
		{
			if (Combination.Keys == KeysEx.None)
				return;

			if (IsEngaged)
				throw new InvalidOperationException("Cannot remove keys while hook is engaged.");

			if (Combination != null && currentHookKeys.Contains(Combination))
				currentHookKeys.Remove(Combination);

			Combination = new KeyCombination(KeysEx.None);
		}

		/// <summary>
		/// Associates the specified key combination with this hook. This can only be performed when the hook is not active.
		/// </summary>
		/// <param name="combination">The key combination.</param>
		/// <exception cref="InvalidOperationException">Occurs if this hook is currently engaged -OR- if the key combination is invalid
		/// -OR- if the key combination is already in use by another hook.</exception>
		public void SetKeys(KeyCombination combination)
		{
			if (Combination == combination)
				return;

			if (IsEngaged)
				throw new InvalidOperationException("Cannot set keys while hook is engaged.");

			if (!combination.IsValid)
				throw new InvalidOperationException("Key combination is not valid.");

			if (currentHookKeys.Contains(combination))
				throw new InvalidOperationException(string.Format("The combination '{0}' is already in use.", combination));

			if (Combination != null && currentHookKeys.Contains(Combination))
				currentHookKeys.Remove(Combination);

			currentHookKeys.Add(combination);
			Combination = combination;
		}

		/// <summary>
		/// Activates the KeyboardHook.
		/// </summary>
		/// <exception cref="InvalidOperationException">Occurs if the KeyboardHook is empty OR if the KeyboardHook is already engaged.</exception>
		/// <exception cref="ObjectDisposedException">Occurs if the KeyboardHook has been disposed.</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Engage()
		{
			if (isDisposed)
				throw new ObjectDisposedException("KeyboardHook");

			if (Combination == null)
				throw new InvalidOperationException("Cannot engage hook when Combination is null.");

			if (IsEngaged)
				return;

			hookId = NativeMethods.SetWindowsHookEx(
				NativeMethods.KeyStateConstants.WH_KEYBOARD_LL,
				hookCallback,
				IntPtr.Zero,
				0);

			if (hookId == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		/// <summary>
		/// Removes the KeyboardHook from the system.
		/// </summary>
		/// <remarks>Disengage removes the hook from the system, but maintains all its data. Use Engage to re-install the hook. To
		/// discard the hook and all its data, use Dispose. It is not necessary to call Disengage prior to Dispose.</remarks>
		public void Disengage()
		{
			if (hookId != IntPtr.Zero)
			{
				NativeMethods.UnhookWindowsHookEx(hookId);
				hookId = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Returns the string representation of the KeyboardHook.
		/// </summary>
		/// <returns>A string representing the KeyboardHook.</returns>
		public override string ToString()
		{
			return Combination == null ? string.Empty : Combination.ToString();
		}

		/// <summary>
		/// Disengages the KeyboardHook and releases all associated resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disengages the KeyboardHook, releases all unmanaged resources, and optionally releases
		/// all managed resources.
		/// </summary>
		/// <param name="isDisposing">True to release both managed and unmanaged resources; false to 
		/// release only unmanaged resources.</param>
		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposed)
				return;

			if (isDisposing)
			{
				// No managed resources to dispose.
			}

			Disengage();

			if (Combination != null)
				currentHookKeys.Remove(Combination);

			isDisposed = true;
		}

		/// <summary>
		/// Raises the Pressed event.
		/// </summary>
		/// <remarks>The Pressed event is executed asynchronously.</remarks>
		protected virtual void OnPressed()
		{
			// Invoke these asychronously in case they're slow. (Testing revealed that Windows will reassert  
			// responsibility for the key stroke(s) if the hookcallback doesn't immediately return.)
			foreach (EventHandler pressedEvent in Pressed.GetInvocationList())
				pressedEvent.BeginInvoke(this, EventArgs.Empty, ar => ((EventHandler)ar.AsyncState).EndInvoke(ar), pressedEvent);
		}

		/// <summary>
		/// Determines whether the specified key, combined with all currently pressed modifier keys, matches this hook's key combination.
		/// </summary>
		/// <param name="unmodifiedKey">The unmodified key to combine with all currently pressed keyboard modifier keys.</param>
		/// <returns>True if unmodifiedKey matches Combination.UnmodifiedKey and all modifier keys contained in the 
		/// Combination property are currently pressed; otherwise, false.</returns>
		protected bool CheckIsHookKeyCombination(KeysEx unmodifiedKey)
		{
			KeyCombination pressedKeyCombo = new KeyCombination(unmodifiedKey);

			if (IsKeyPressed(KeysEx.ControlKey))
				pressedKeyCombo.Add(KeysEx.Control);

			if (IsKeyPressed(KeysEx.ShiftKey))
				pressedKeyCombo.Add(KeysEx.Shift);

			if (IsKeyPressed(KeysEx.LMenu) || IsKeyPressed(KeysEx.RMenu))
				pressedKeyCombo.Add(KeysEx.Alt);

			if ((IsKeyPressed(KeysEx.LWin) || IsKeyPressed(KeysEx.RWin)))
				pressedKeyCombo.Add(KeysEx.WinLogo);

			return Combination == pressedKeyCombo;
		}

		/// <summary>
		/// The callback proceedure for the installed KeyboardHook. 
		/// </summary>
		/// <param name="nCode">A code the KeyboardHook procedure uses to determine how to process the message.</param>
		/// <param name="wParam">The key state.</param>
		/// <param name="lParam">The key pressed.</param>
		/// <returns>A value indicating whether or not to process additional hooks in the current hook chain.</returns>
		IntPtr HookCallBack(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (isKeyboardLockedDown)
				return new IntPtr(1); // A non-zero return value blocks additional processing of key strokes.

			// MSDN documentation indicates that nCodes less than 0 should always only invoke CallNextHookEx.
			if (nCode < 0)
				return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);

			int keyStateParam = (int)wParam;
			KeyState keyState;

			if (keyStateParam == keyUp || keyStateParam == systemKeyUp)
				keyState = KeyState.Up;
			else if (keyStateParam == keyDown || keyStateParam == systemKeyDown)
				keyState = KeyState.Down;
			else
				throw new ArgumentOutOfRangeException("wParam", "Invalid key state detected.");

			var pressedKey = (KeysEx)Marshal.ReadInt32(lParam);

			if (!CheckIsHookKeyCombination(pressedKey))
				return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);

			if (keyState == KeyState.Up)
			{
				keyReleased = true;
				return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
			}

			// If AutoRepeat is on, always process hook; otherwise, only process hook if they key has been released and re-pressed.
			if (AutoRepeat || keyReleased)
			{
				keyReleased = false;
				OnPressed();
			}

			if (AllowPassThrough)
				return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);

			return new IntPtr(1);
		}
	}
}
