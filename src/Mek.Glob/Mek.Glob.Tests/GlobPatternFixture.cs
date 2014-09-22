using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mek.Glob.Tests
{
	[TestFixture]
    public class GlobPatternFixture
    {
		[TestCase("*")]
		[TestCase("?")]
		[TestCase("**")]
		public void should_have_glob_pattern(string pattern)
		{
			Assert.IsTrue(GlobPattern.HasPattern(pattern));
		}

		[TestCase(@"\*")]
		[TestCase(@"\?")]
		[TestCase(@"\**")]
		public void should_not_have_glob_pattern(string pattern)
		{
			Assert.IsFalse(GlobPattern.HasPattern(pattern));
		}

		[TestCase("*", "test")]
		[TestCase("**", "test")]
		[TestCase("****", "test")]
		[TestCase("****A", "testA")]
		[TestCase("?", "a")]
		[TestCase("??", "aa")]
		[TestCase("???", "aaa")]
		[TestCase("hello", "hello")]
		public void should_be_glob_match(string pattern, string text)
		{
			Assert.IsTrue(GlobPattern.IsMatch(pattern, text));
		}
    }
}
