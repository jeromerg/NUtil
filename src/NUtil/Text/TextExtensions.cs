using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NUtil.Linq;

namespace NUtil.Text
{
    public static class TextExtensions
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<string> Lines([NotNull] this string s)
        {
            if (s == null) throw new ArgumentNullException("s");
            string[] lines = s.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            return lines;
        }

        [NotNull]
        public static string JoinLines([NotNull, ItemCanBeNull] this IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            return string.Join(Environment.NewLine, lines);
        }

        [NotNull]
        public static string Desindent([NotNull] this string txt, int tabIndentation)
        {
            if (txt == null) throw new ArgumentNullException("txt");

            string preprocessedTxt = ConvertTabToSpace(txt, tabIndentation);

            string[] lines = preprocessedTxt.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            // ReSharper disable once AssignNullToNotNullAttribute
            int indentMin = lines.Min(line => GetIndent(line));

            string desindentedTxt = lines
                // ReSharper disable once PossibleNullReferenceException
                .Select(line => line.Length >= indentMin ? line.Substring(indentMin) : "")
                .JoinLines();

            return desindentedTxt;
        }

        [NotNull]
        public static string Indent([NotNull] this string txt, int indentation)
        {
            if (txt == null) throw new ArgumentNullException("txt");

            var sb = new StringBuilder();

            txt.Lines().ForEach(l =>
                                {
                                    sb.Append(new string(' ', indentation));
                                    sb.Append(l);
                                },
                                () => sb.AppendLine());

            return sb.ToString();
        }

        private static int GetIndent([NotNull] string s)
        {
            int result = 0;
            foreach (char c in s)
            {
                if (c == ' ')
                    result += 1;
                else
                    return result;
            }
            return int.MaxValue;
        }

        [NotNull]
        private static string ConvertTabToSpace([NotNull] this string txt, int tabIndentation)
        {
            string spaces = new string(' ', tabIndentation);
            string result = txt.Replace("\t", spaces);
            return result;
        }

   }
}