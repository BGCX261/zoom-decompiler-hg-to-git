﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiffLib;
using System.Diagnostics;

namespace Mi.Decompiler.Tests.Helpers
{
	public class CodeAssert
	{
		public static void AreEqual(string input1, string input2)
		{
			var diff = new StringWriter();
			if (!Compare(input1, input2, diff)) {
				throw new MatchException(input1, input2, diff.ToString());
			}
		}

		static bool Compare(string input1, string input2, StringWriter diff)
		{
            string[] inputLines1 = NormalizeAndSplitCode(input1);
            string[] inputLines2 = NormalizeAndSplitCode(input2);

			var differ = new AlignedDiff<string>(
				inputLines1,
                inputLines2,
				new CodeLineEqualityComparer(),
				new StringSimilarityComparer(),
				new StringAlignmentFilter());

			bool result = true, ignoreChange;

			int line1 = 0, line2 = 0;

			foreach (var change in differ.Generate()) {
				switch (change.Change) {
					case ChangeType.Same:
						diff.Write("{0,4} {1,4} ", ++line1, ++line2);
						diff.Write("  ");
						diff.WriteLine(change.Element1);
						break;
					case ChangeType.Added:
						diff.Write("     {1,4} ", line1, ++line2);
						result &= ignoreChange = ShouldIgnoreChange(change.Element2);
						diff.Write(ignoreChange ? "    " : " +  ");
						diff.WriteLine(change.Element2);
						break;
					case ChangeType.Deleted:
						diff.Write("{0,4}      ", ++line1, line2);
						result &= ignoreChange = ShouldIgnoreChange(change.Element1);
						diff.Write(ignoreChange ? "    " : " -  ");
						diff.WriteLine(change.Element1);
						break;
					case ChangeType.Changed:
						diff.Write("{0,4}      ", ++line1, line2);
						result = false;
						diff.Write("(-) ");
						diff.WriteLine(change.Element1);
						diff.Write("     {1,4} ", line1, ++line2);
						diff.Write("(+) ");
						diff.WriteLine(change.Element2);
						break;
				}
			}

			return result;
		}

		class CodeLineEqualityComparer : IEqualityComparer<string>
		{
			private IEqualityComparer<string> baseComparer = EqualityComparer<string>.Default;

			public bool Equals(string x, string y)
			{
				return baseComparer.Equals(
					NormalizeLine(x),
					NormalizeLine(y)
				);
			}

			public int GetHashCode(string obj)
			{
				return baseComparer.GetHashCode(NormalizeLine(obj));
			}
		}

		private static string NormalizeLine(string line)
		{
			line = line.Trim();
			var index = line.IndexOf("//");
			if (index >= 0) {
				return line.Substring(0, index);
			} else {
				return line;
			}
		}

		private static bool ShouldIgnoreChange(string line)
		{
			// for the result, we should ignore blank lines and added comments
			return NormalizeLine(line) == string.Empty;
		}

		private static string[] NormalizeAndSplitCode(string input)
		{
			return input.Split(new[] { "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.None);
		}
	}
}
