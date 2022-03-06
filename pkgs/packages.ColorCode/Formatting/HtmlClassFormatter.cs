using System;
using System.Collections.Generic;
using System.IO;
using packages.ColorCode.Common;
using packages.ColorCode.Parsing;

namespace packages.ColorCode.Formatting
{
    public class HtmlClassFormatter : IFormatter
    {
        public void Write(string parsedSourceCode,
                          IList<Scope> scopes,
                          IStyleSheet styleSheet,
                          TextWriter textWriter)
        {
            var styleInsertions = new List<TextInsertion>();

            foreach (Scope scope in scopes)
                GetStyleInsertionsForCapturedStyle(scope, styleInsertions);

            styleInsertions.SortStable((x, y) => x.Index.CompareTo(y.Index));

            int offset = 0;

            foreach (TextInsertion styleInsertion in styleInsertions)
            {
                textWriter.Write(HttpUtility.HtmlEncode(parsedSourceCode[offset..styleInsertion.Index]));
                if (string.IsNullOrEmpty(styleInsertion.Text))
                    BuildSpanForCapturedStyle(styleInsertion.Scope, styleSheet, textWriter);
                else
                    textWriter.Write(styleInsertion.Text);
                offset = styleInsertion.Index;
            }

            textWriter.Write(HttpUtility.HtmlEncode(parsedSourceCode[offset..]));
        }

        public void WriteFooter(IStyleSheet styleSheet,
                                ILanguage language,
                                TextWriter textWriter)
        {
            Guard.ArgNotNull(styleSheet, nameof(styleSheet));
            Guard.ArgNotNull(language, nameof(language));
            Guard.ArgNotNull(textWriter, nameof(textWriter));

            textWriter.WriteLine();
            WriteHeaderPreEnd(textWriter);
            WriteHeaderDivEnd(textWriter);
        }

        public void WriteHeader(IStyleSheet styleSheet,
                                ILanguage language,
                                TextWriter textWriter)
        {
            Guard.ArgNotNull(styleSheet, nameof(styleSheet));
            Guard.ArgNotNull(language, nameof(language));
            Guard.ArgNotNull(textWriter, nameof(textWriter));

            WriteHeaderDivStart(language, textWriter);
            WriteHeaderPreStart(textWriter);
            textWriter.WriteLine();
        }

        private static void GetStyleInsertionsForCapturedStyle(Scope scope, ICollection<TextInsertion> styleInsertions)
        {
            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index,
                Scope = scope
            });


            foreach (Scope childScope in scope.Children)
                GetStyleInsertionsForCapturedStyle(childScope, styleInsertions);

            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index + scope.Length,
                Text = "</span>"
            });
        }

        private static void BuildSpanForCapturedStyle(Scope scope,
                                                        IStyleSheet styleSheet,
                                                        TextWriter writer)
        {
            string cssClassName = "";

            if (styleSheet.Styles.Contains(scope.Name))
            {
                Style style = styleSheet.Styles[scope.Name];

                cssClassName = style.CssClassName;
            }

            WriteElementStart("span", cssClassName, writer);
        }

        private static void WriteHeaderDivEnd(TextWriter writer)
        {
            WriteElementEnd("div", writer);
        }

        private static void WriteElementEnd(string elementName,
                                            TextWriter writer)
        {
            writer.Write("</{0}>", elementName);
        }

        private static void WriteHeaderPreEnd(TextWriter writer)
        {
            WriteElementEnd("pre", writer);
        }

        private static void WriteHeaderPreStart(TextWriter writer)
        {
            WriteElementStart("pre", writer);
        }

        private static void WriteHeaderDivStart(ILanguage language,
                                                TextWriter writer)
        {
            WriteElementStart("div", language.CssClassName, writer);
        }

        private static void WriteElementStart(string elementName,
                                              TextWriter writer)
        {
            WriteElementStart(elementName, "", writer);
        }

        private static void WriteElementStart(string elementName,
                                              string cssClassName,
                                              TextWriter writer)
        {
            writer.Write("<{0}", elementName);
            if (!String.IsNullOrEmpty(cssClassName)) {
                writer.Write(" class=\"{0}\"", cssClassName);
            }
            writer.Write(">");
        }
    }
}
