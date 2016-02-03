using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using NUtil.Linq;

namespace NUtil.Test.Linq
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "IteratorMethodResultIsIgnored")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
    public class CascadeExtensionsTests
    {
        [Test]
        public void CascadeAdd_FirstArgNull()
        {
            Dictionary<int, int> dict = null;
            Assert.Throws<ArgumentNullException>(() => dict.CascadeAdd(12));
        }

        [Test]
        public void CascadeAdd_SecondArgNull()
        {
            var dict = new Dictionary<string, int>();
            Assert.Throws<ArgumentNullException>(() => dict.CascadeAdd(null));
        }

        [Test]
        public void CascadeRemove_FirstArgNull()
        {
            Dictionary<int, IEnumerable<string>> dict = null;
            Assert.Throws<ArgumentNullException>(() => dict.CascadeRemove(12));
        }

        [Test]
        public void CascadeRemove2_FirstArgNull()
        {
            IEnumerable<Dictionary<int, IEnumerable<string>>> dictEnum = null;
            Assert.Throws<ArgumentNullException>(() =>
                {
                    var e = CascadeExtensions.CascadeRemove(dictEnum, 12);
                    e.GetEnumerator().MoveNext();
                });
        }

        [Test]
        public void CascadeRemove2_SecondArgNull()
        {
            var dictEnum = new List<Dictionary<string, IEnumerable<int>>>();
            Assert.Throws<ArgumentNullException>(() =>
                {
                    var e = CascadeExtensions.CascadeRemove(dictEnum, null);
                    e.GetEnumerator().MoveNext();
                });
        }
    }
}
