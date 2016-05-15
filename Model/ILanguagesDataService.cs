using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkingAlphabets.Model
{
    public interface ILanguagesDataService 
    {
        Task<ObservableCollection<Language>> GetLanguagesAsync();

        Task<bool> AddNewLanguageAsync(Language language);

        Task<bool> DeleteLanguageAsync(Language language);

        Task<Language> GetSelectedLanguageAsync();

        bool SelectNewLanguage(Language language);
    }
}
