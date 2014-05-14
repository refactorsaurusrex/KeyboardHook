using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyboardHookReduxSample
{
	/// <summary>
	/// Represents a keyboard key combination.
	/// </summary>
	public class KeyCombination : IEquatable<KeyCombination>
	{
		/// <summary>
		/// The backing keys for this combination.
		/// </summary>
		KeysEx keyChain;

		/// <summary>
		/// Creates a new KeyCombination based on the provided archetype. The new combination is a deep copy
		/// of the original.
		/// </summary>
		/// <param name="combo">The combination to copy from.</param>
		/// <returns>A new KeyCombination object.</returns>
		public static KeyCombination CopyFrom(KeyCombination combo)
		{
			return new KeyCombination(combo.Keys);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyCombination"/> class.
		/// </summary>
		/// <param name="keys">The keys that constitute a keyboard combination.</param>
		public KeyCombination(Keys keys)
		{
			keyChain = keys.ToKeysEx();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyCombination"/> class.
		/// </summary>
		/// <param name="keys">The keys that constitute a keyboard combination.</param>
		public KeyCombination(KeysEx keys)
		{
			keyChain = keys;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyCombination"/> class.
		/// </summary>
		/// <param name="keys">The keys that constitute a keyboard combination, express in the following
		/// format: "Modifer + Modifier + Key".</param>
		public KeyCombination(string keys)
		{
			if (!string.IsNullOrEmpty(keys))
				FromStringToKeys(keys);
		}

		/// <summary>
		/// Returns true if the values of its operands are equal, otherwise false.
		/// </summary>
		public static bool operator ==(KeyCombination x, KeyCombination y)
		{
			if (ReferenceEquals(null, x))
				return ReferenceEquals(null, y);

			return x.Equals(y);
		}

		/// <summary>
		/// returns false if its operands are equal, otherwise true.
		/// </summary>
		public static bool operator !=(KeyCombination x, KeyCombination y)
		{
			return !(x == y);
		}

		/// <summary>
		/// Adds the specified keys to this combination. Note that a combination can only contain a single
		/// non-modifier key. If one already exists, it will be overwritten.
		/// </summary>
		/// <param name="keys">The keys to add.</param>
		public void Add(KeysEx keys)
		{
			KeysEx keyCode = keys & KeysEx.KeyCode;

			if (keyCode != KeysEx.None)
				keyChain = keyCode | keyChain & KeysEx.Modifiers;

			KeysEx modifiers = keys & KeysEx.Modifiers;

			if (modifiers != KeysEx.None)
				keyChain = modifiers | keyChain & KeysEx.KeyCode | keyChain & KeysEx.Modifiers;
		}

		/// <summary>
		/// Removes the specified key from this combination. Must be called once for each key to be removed.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		public void Remove(KeysEx key)
		{
			if (Contains(key))
				keyChain ^= key;

			if (key == KeysEx.LWin || key == KeysEx.RWin)
			{
				if (Contains(KeysEx.WinLogo))
					keyChain ^= KeysEx.WinLogo;
			}
		}

		/// <summary>
		/// Determines whether the specified key exists in this combination.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>True if the specified key exists; otherwise, false.</returns>
		public bool Contains(KeysEx key)
		{
			switch (key)
			{
				case KeysEx.Control:
				case KeysEx.Shift:
				case KeysEx.Alt:
				case KeysEx.WinLogo:
					return (keyChain & KeysEx.Modifiers) == key;

				default:
					return (keyChain & KeysEx.KeyCode) == key;
			}
		}

		/// <summary>
		/// Gets the keys that constitute this combination.
		/// </summary>
		public KeysEx Keys
		{
			get { return keyChain; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return Keys == KeysEx.None; }
		}

		/// <summary>
		/// Gets the unmodified key for this combination.
		/// </summary>
		public KeysEx UnmodifiedKey
		{
			get { return keyChain & KeysEx.KeyCode; }
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString()
		{
			return keyChain.ToFriendlyString();
		}

		/// <summary>
		/// Gets a value indicating whether this combination is valid.
		/// </summary>
		/// <remarks>A valid combination is one which can be used as a keyboard shortcut. Certain 
		/// keys such as Enter or CapsLock cannot be used for a shortcut. A combination also cannot 
		/// consist solely of modifier keys.</remarks>
		public bool IsValid
		{
			get
			{
				KeysEx keyCode = keyChain & KeysEx.KeyCode;
				switch (keyCode)
				{
					case KeysEx.Enter:
					case KeysEx.CapsLock:
					case KeysEx.NumLock:
					case KeysEx.Tab:
					case KeysEx.None:
					case KeysEx.ShiftKey:
					case KeysEx.LShiftKey:
					case KeysEx.RShiftKey:
					case KeysEx.ControlKey:
					case KeysEx.LControlKey:
					case KeysEx.RControlKey:
					case KeysEx.Menu:
					case KeysEx.LMenu:
					case KeysEx.RMenu:
					case KeysEx.LWin:
					case KeysEx.RWin:
						return false;

					case KeysEx.Delete:
						if ((keyChain & KeysEx.Modifiers) == (KeysEx.Control | KeysEx.Alt))
							return false;
						break;
				}

				return true;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as KeyCombination);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return 17 * 23 + ToString().GetHashCode();
			}
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
		public bool Equals(KeyCombination other)
		{
			if (ReferenceEquals(null, other))
				return false;

			return ToString() == other.ToString();
		}

		/// <summary>
		/// Froms the string to keys.
		/// </summary>
		/// <param name="keys">The keys.</param>
		void FromStringToKeys(string keys)
		{
			IEnumerable<string> segments = keys.Split(new[] { '+' });

			foreach (string segment in segments)
			{
				string modifiedSegment = segment.ToLowerInvariant().Trim() == "ctrl" ? "Control" : segment;
				keyChain |= EnumParser.Parse<KeysEx>(modifiedSegment.Trim());
			}
		}
	}
}
