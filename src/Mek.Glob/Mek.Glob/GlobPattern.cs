using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mek.Glob
{
	public class GlobPattern
	{
		private static int IndexNotOf(string str, int startIndex, char c)
		{
			if (string.IsNullOrWhiteSpace(str))
				return -1;

			if (startIndex < 0)
				throw new ArgumentException("start index must be greater than 0", "startIndex");

			for (var i = startIndex; i < str.Length; i++)
			{
				if (str[i] != c)
					return i;
			}

			return -1;
		}

		public static bool IsFileSystemCaseSensitive
		{
			get
			{
				switch (Environment.OSVersion.Platform)
				{
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
					case PlatformID.Xbox:
						return false;

					default:
						return true;
				}
			}
		}

		public static bool HasPattern(string globPattern)
		{
			var chars = new[] { '*', '?', '{', '}', '[', ']' };
			var charIndex = globPattern.IndexOfAny(chars);
			var prevIndex = charIndex - 1;

			if (charIndex == 0)
			{
				return true;
			}
			else if (charIndex >= 0 && prevIndex >= 0 && globPattern[prevIndex] != '\\')
			{
				return true;
			}

			return false;
		}

		public static Regex GetRegex(string globPattern)
		{
			if (string.IsNullOrWhiteSpace(globPattern))
				throw new ArgumentException("globPattern is empty or null", "globPattern");

			var sb = new StringBuilder();

			sb.Append("^");

			for (var charIndex = 0; charIndex < globPattern.Length; charIndex++)
			{
				var currentChar = globPattern[charIndex];

				if (currentChar == '*')
				{
					sb.Append(".*");

					var nextIndex = IndexNotOf(globPattern, charIndex, '*');

					if (nextIndex == -1)
						break;
					else
						charIndex = nextIndex - 1;
				}
				else if (currentChar == '?')
				{
					sb.Append('.');
				}
				else
				{
					sb.Append(Regex.Escape(currentChar.ToString()));
				}
			}

			sb.Append("$");

			var opts = RegexOptions.Singleline;

			if (!IsFileSystemCaseSensitive)
				opts = opts | RegexOptions.IgnoreCase;

			var rx = new Regex(sb.ToString(), opts);
			return rx;
		}

		public static bool IsMatch(string pattern, string text)
		{
			return new GlobPattern(pattern).IsMatch(text);
		}

		private string pattern;
		private Regex patternRx;

		public GlobPattern(string pattern)
		{
			if(string.IsNullOrWhiteSpace(pattern))
				throw new ArgumentException("pattern is empty or null", "pattern");

			if(HasPattern(pattern))
			{
				this.patternRx = GetRegex(pattern);
			}
			else
			{
				this.pattern = pattern;
			}
		}

		public bool IsMatch(string text)
		{
			if(this.patternRx != null)
			{
				return this.patternRx.IsMatch(text);
			}
			else
			{
				var comparisonType = IsFileSystemCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
				return string.Equals(this.pattern, text, comparisonType);
			}
		}
	}
}
