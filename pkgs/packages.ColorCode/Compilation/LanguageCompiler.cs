using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using packages.ColorCode.Common;

namespace packages.ColorCode.Compilation
{
    public class LanguageCompiler : ILanguageCompiler
    {
        private static readonly Regex numberOfCapturesRegex = new(@"(?x)(?<!(\\|(?!\\)\(\?))\((?!\?)", Compiled());
        private readonly Dictionary<string, CompiledLanguage> compiledLanguages;
        private readonly ReaderWriterLockSlim compileLock;

        public LanguageCompiler(Dictionary<string, CompiledLanguage> compiledLanguages)
        {
            this.compiledLanguages = compiledLanguages;

            compileLock = new ReaderWriterLockSlim();
        }

        public CompiledLanguage Compile(ILanguage language)
        {
            Guard.ArgNotNull(language, nameof(language));

            if (string.IsNullOrEmpty(language.Id))
                throw new ArgumentException("The language identifier must not be null.", nameof(language));
            
            CompiledLanguage compiledLanguage;

            compileLock.EnterReadLock();
            try
            {
                // for performance reasons we should first try with
                // only a read lock since the majority of the time
                // it'll be created already and upgradeable lock blocks
                if (compiledLanguages.ContainsKey(language.Id))
                    return compiledLanguages[language.Id];
            }
            finally
            {
                compileLock.ExitReadLock();
            }

            compileLock.EnterUpgradeableReadLock();
            try
            {
                if (compiledLanguages.ContainsKey(language.Id))
                    compiledLanguage = compiledLanguages[language.Id];
                else
                {
                    compileLock.EnterWriteLock();

                    try
                    {
                        if (string.IsNullOrEmpty(language.Name))
                            throw new ArgumentException("The language name must not be null or empty.", nameof(language));
                        
                        if (language.Rules == null || language.Rules.Count == 0)
                            throw new ArgumentException("The language rules collection must not be empty.", nameof(language));
                        
                        compiledLanguage = CompileLanguage(language);

                        compiledLanguages.Add(compiledLanguage.Id, compiledLanguage);
                    }
                    finally
                    {
                        compileLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                compileLock.ExitUpgradeableReadLock();
            }

            return compiledLanguage;
        }

        private static RegexOptions Compiled()
        {
            if (Enum.TryParse("Compiled", out RegexOptions compiledOption))
            {
                return compiledOption;
            }
            return RegexOptions.None;
        }

        private static CompiledLanguage CompileLanguage(ILanguage language)
        {
            string id = language.Id;
            string name = language.Name;

            CompileRules(language.Rules, out Regex regex, out IList<string> captures);

            return new CompiledLanguage(id, name, regex, captures);
        }

        private static void CompileRules(IList<LanguageRule> rules,
                                         out Regex regex,
                                         out IList<string> captures)
        {
            StringBuilder regexBuilder = new();
            captures = new List<string>();

            regexBuilder.AppendLine("(?x)");
            captures.Add(null);

            CompileRule(rules[0], regexBuilder, captures, true);

            for (int i = 1; i < rules.Count; i++)
                CompileRule(rules[i], regexBuilder, captures, false);

            regex = new Regex(regexBuilder.ToString());
        }


        private static void CompileRule(LanguageRule languageRule,
                                                 StringBuilder regex,
                                                 ICollection<string> captures,
                                                 bool isFirstRule)
        {
            if (!isFirstRule)
            {
                regex.AppendLine();
                regex.AppendLine();
                regex.AppendLine("|");
                regex.AppendLine();
            }
            
            regex.AppendFormat("(?-xis)(?m)({0})(?x)", languageRule.Regex);

            int numberOfCaptures = GetNumberOfCaptures(languageRule.Regex);

            for (int i = 0; i <= numberOfCaptures; i++)
            {
                string scope = null;

                foreach (int captureIndex in languageRule.Captures.Keys)
                {
                    if (i == captureIndex)
                    {
                        scope = languageRule.Captures[captureIndex];
                        break;
                    }
                }

                captures.Add(scope);
            }
        }

        private static int GetNumberOfCaptures(string regex)
        {
            return numberOfCapturesRegex.Matches(regex).Count;
        }
    }
}