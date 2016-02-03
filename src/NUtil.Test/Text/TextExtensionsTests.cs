using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using NUtil.Text;

namespace NUtil.Test.Text
{
    [TestFixture]
    [SuppressMessage("ReSharper", "IteratorMethodResultIsIgnored")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class TextExtensionsTests
    {
        private const string JOINED_LINES = "\r\na\r\nb\r\n\r\n";
        private readonly string [] mLines = new string[] {"", "a", "b", "", ""};

        [Test]
        public void Lines_InputNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).Lines());
        }

        [Test]
        public void LinesTest()
        {
            Assert.AreEqual(mLines, JOINED_LINES.Lines());
        }

        [Test]
        public void JoinLines_InputNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((string[])null).JoinLines());
        }

        [Test]
        public void JoinLinesTest_including_emptyLines()
        {
            Assert.AreEqual(JOINED_LINES, mLines.JoinLines());
        }

        [Test]
        public void Desindent_InputNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).Desindent(4));
        }

        [Test]
        public void Desindent_OnlyNewLine()
        {
            string i = @"\n";
            string o = @"\n";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Desindent_EmptyString()
        {
            string i = @"";
            string o = @"";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Desindent_OneLine()
        {
            string i = "a";
            string o = "a";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Desindent_OneIndentedLine()
        {
            string i = "  a";
            string o = "a";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Desindent_OneIndentedLine_Tab()
        {
            string i = "\ta";
            string o = "a";
            Assert.AreEqual(o, i.Desindent(4));
        }


        [Test]
        public void Desindent_TwoLines()
        {
            string i = "a\r\n  b";
            string o = "a\r\n  b";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Desindent_TwoIndentedLines()
        {
            string i = " a\r\n  b";
            string o = "a\r\n b";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Desindent_TwoIndentedLines_oneWithTab()
        {
            string i = " a\r\n\tb";
            string o = "a\r\n   b";
            Assert.AreEqual(o, o.Desindent(4));
        }

        [Test]
        public void Desindent_TwoIndentedLines_oneWithTabLimits()
        {
            string i = "     a\r\n\tb";
            string o = " a\r\nb";
            Assert.AreEqual(o, i.Desindent(4));
        }

        [Test]
        public void Indent_InputNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null).Indent(4));
        }

        [Test]
        public void Indent_TwoLines()
        {
            string i = "a\r\n  b";
            string o = "     a\r\n       b";
            Assert.AreEqual(o, i.Indent(5));
        }

        [Test]
        public void Indent_OnlyNewLine()
        {
            string i = @"\n";
            string o = @"     \n";
            Assert.AreEqual(o, i.Indent(5));
        }




  }
}