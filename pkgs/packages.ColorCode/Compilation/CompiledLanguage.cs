using System.Collections.Generic;
using System.Text.RegularExpressions;
using packages.ColorCode.Common;

namespace packages.ColorCode.Compilation
{
    public class CompiledLanguage
    {
        public CompiledLanguage(string id,
                                string name,
                                Regex regex,
                                IList<string> captures)
        {
            Guard.ArgNotNullAndNotEmpty(id, nameof(id));
            Guard.ArgNotNullAndNotEmpty(name, nameof(name));
            Guard.ArgNotNull(regex, nameof(regex));
            Guard.ArgNotNullAndNotEmpty(captures, nameof(captures));
            
            Id = id;
            Name = name;
            Regex = regex;
            Captures = captures;
        }

        public IList<string> Captures { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Regex Regex { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}