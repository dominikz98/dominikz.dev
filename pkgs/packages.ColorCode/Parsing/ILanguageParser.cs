using System;
using System.Collections.Generic;

namespace packages.ColorCode.Parsing
{
    public interface ILanguageParser
    {
        void Parse(string sourceCode,
                   ILanguage language,
                   Action<string, IList<Scope>> parseHandler);
    }
}