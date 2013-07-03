using FreeEverything;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FreeEverythingTest
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void ShouldSkipReturenFalseIfEmpty()
        {
            Filter filter = new Filter();
            Assert.IsFalse(filter.ShouldSkip(@"d:\abc"));
        }

        [TestMethod]
        public void ShouldSkipReturenFalseIfNotInExcludeFolderAndIncludeFolderIsEmpty()
        {
            Filter filter = new Filter {Exclude = @"d:\abcd"};
            Assert.IsFalse(filter.ShouldSkip(@"d:\abc\a"));
        }

        [TestMethod]
        public void ShouldSkipReturenFalseIfNotInExcludeFolderAndInIncludeFolder()
        {
            Filter filter = new Filter {Exclude = @"d:\abcd", Include = @"d:\abc"};
            Assert.IsFalse(filter.ShouldSkip(@"d:\abc\a"));
        }

        [TestMethod]
        public void ShouldSkipReturenTrueIfNotInExcludeFolderAndNotInIncludeFolder()
        {
            Filter filter = new Filter {Exclude = @"d:\abcd", Include = @"d:\abce"};
            Assert.IsTrue(filter.ShouldSkip(@"d:\abc\a"));
        }

        [TestMethod]
        public void ShouldSkipReturenTrueIfInExcludeFolderAndInIncludeFolder()
        {
            Filter filter = new Filter {Exclude = @"d:\abc", Include = @"d:\abc"};
            Assert.IsTrue(filter.ShouldSkip(@"d:\abc\a"));
        }

        [TestMethod]
        public void ShouldSkipReturenFalseIfNotInExcludeFolders()
        {
            Filter filter = new Filter {Exclude = @"d:\abcd,d:\abce,d:\abcf"};
            Assert.IsFalse(filter.ShouldSkip(@"d:\abc\a"));
        }

        [TestMethod]
        public void ShouldSkipReturenFalseIfInIncludeFolders()
        {
            Filter filter = new Filter {Include = @"d:\abcd,d:\abce,d:\abc"};
            Assert.IsFalse(filter.ShouldSkip(@"d:\abc\a"));
        }

        [TestMethod]
        public void FormatWhenSetInclude()
        {
            Filter filter = new Filter {Include = @"  d:\abcd\\\\\, d:\ab ;d:\e  "};
            Assert.AreEqual(@"d:\abcd\,d:\ab\,d:\e\,", filter.Include);
        }
    }
}
