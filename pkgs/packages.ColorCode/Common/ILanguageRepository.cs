using System.Collections.Generic;

namespace packages.ColorCode.Common
{
    public interface ILanguageRepository
    {
        IEnumerable<ILanguage> All { get; }
        ILanguage FindById(string languageId);
        void Load(ILanguage language);
    }
}