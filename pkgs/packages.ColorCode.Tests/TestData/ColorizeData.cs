using packages.ColorCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace packages.ColorCode.Tests.TestData
{
    public class ColorizeData : DataAttribute
    {
        readonly Regex sourceFileRegex = new(@"(?i)[a-z]+\.source\.([a-z0-9]+)", RegexOptions.Compiled);

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            var colorizeData = new List<object[]>();

            string appPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);

            string[] dirNames = Directory.GetDirectories(Path.Combine(appPath, "Data"));

            foreach (string dirName in dirNames)
            {
                string[] sourceFileNames = Directory.GetFiles(dirName, "*.source.*");

                foreach (string sourceFileName in sourceFileNames)
                {
                    Match sourceFileMatch = sourceFileRegex.Match(sourceFileName);

                    if (sourceFileMatch.Success)
                    {
                        string fileExtension = sourceFileMatch.Groups[1].Captures[0].Value;
                        string languageId = GetLanguageId(fileExtension);

                        string expectedFileName = sourceFileName.Replace(".source.", ".expected.").Replace("." + fileExtension, ".html");

                        colorizeData.Add(new object[] { languageId, sourceFileName, expectedFileName });
                    }
                }
            }

            return colorizeData;
        }

        private static string GetLanguageId(string fileExtension)
        {
            return fileExtension switch
            {
                "asax" => LanguageId.Asax,
                "ashx" => LanguageId.Ashx,
                "cs" => LanguageId.CSharp,
                "php" => LanguageId.Php,
                "css" => LanguageId.Css,
                "vb" => LanguageId.VbDotNet,
                "sql" => LanguageId.Sql,
                "xml" => LanguageId.Xml,
                "ps1" => LanguageId.PowerShell,
                "ts" => LanguageId.TypeScript,
                "fs" => LanguageId.FSharp,
                _ => throw new ArgumentException(string.Format("Unexpected file extension: {0}.", fileExtension)),
            };
        }
    }
}