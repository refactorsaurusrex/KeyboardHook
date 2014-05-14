using System;

namespace KeyboardHookReduxSample
{
	/// <summary>
	/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
	/// </summary>
	public static class EnumParser
	{
		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
		/// </summary>
		/// <typeparam name="T">The enum type.</typeparam>
		/// <param name="value">A string containing the name or value to convert.</param>
		public static T Parse<T>(string value) where T : struct
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
		/// </summary>
		/// <typeparam name="T">The enum type.</typeparam>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <param name="ignoreCase">If true, ignore case; otherwise, regard case.</param>
		public static T Parse<T>(string value, bool ignoreCase) where T : struct
		{
			return (T)Enum.Parse(typeof(T), value, ignoreCase);
		}
	}
}