using packages.ColorCode.Tests.TestData;
using Xunit;

namespace packages.ColorCode.Tests
{
    public class CppAcceptanceTests
    {
        public class CommentTests
        {
            [Fact]
            public void WillColorizeACommentOnMultipleLines()
            {
                const string source = @"/*
comment line
comment line 2
*/";
                var expected = AcceptanceHelper.BuildExpected(@"<span style=""color:Green;"">/*
comment line
comment line 2
*/</span>");

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WillColorizeAMultieLineStyleCommentOnOneLine()
            {
                const string source = @"/*comment*/";
                var expected = AcceptanceHelper.BuildExpected(@"<span style=""color:Green;"">/*comment*/</span>");

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void WillColorizeASingleLineStyleComment()
            {
                const string source = @"//comment";
                var expected = AcceptanceHelper.BuildExpected(@"<span style=""color:Green;"">//comment</span>");

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(expected, actual);
            }
        }

        public class StringTests
        {
            [Fact]
            public void WillColorizeStrings()
            {
                const string source = @"string aString = ""aString"";";
                var expected = AcceptanceHelper.BuildExpected(@"string aString = <span style=""color:#A31515;"">&quot;aString&quot;</span>;");

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(expected, actual);
            }
        }
        
        public class KeywordTests
        {
            [Theory]
            [CppKeywordData]
            public void WillColorizeAKeywordWithNoSurroundingText(string keyword)
            {
                var source = keyword;
                var exepected = AcceptanceHelper.BuildExpected(@"<span style=""color:Blue;"">{0}</span>", keyword);

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(exepected, actual);
            }

            [Theory]
            [CppKeywordData]
            public void WillColorizeAKeywordWithPrecedingAndSucceedingText(string keyword)
            {
                var source = string.Format("fnord {0} fnord", keyword);
                var exepected = AcceptanceHelper.BuildExpected(@"fnord <span style=""color:Blue;"">{0}</span> fnord", keyword);

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(exepected, actual);
            }

            [Theory]
            [CppKeywordData]
            public void WillNotColorizeAKeywordInsideAWord(string keyword)
            {
                var source = string.Format("fnord{0}fnord", keyword);
                var exepected = AcceptanceHelper.BuildExpected(@"fnord{0}fnord", keyword);

                var actual = new CodeColorizer().Colorize(source, Languages.Cpp);

                Assert.Equal(exepected, actual);
            }
        }
    }
}
