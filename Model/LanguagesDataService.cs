using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InkingAlphabets.DB;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.IO;
using Microsoft.Data.Entity;

namespace InkingAlphabets.Model
{
    class LanguagesDataService : ILanguagesDataService
    {
        const string SelectedLanguageKey = "SelectedLanguage";
        ObservableCollection<Language> _languages;
        public async Task<bool> AddNewLanguageAsync(Language language)
        {
            bool status = false;
            try
            {
                using (var db = new SQLiteDbContext())
                {
                    //Delete if records exists in the DB
                    if (db.Languages.Any(l => l.LanguageName.Equals(language.LanguageName)))
                    {
                        db.Languages.Remove(db.Languages.First(l => l.LanguageName.Equals(language.LanguageName)));
                        await db.SaveChangesAsync();
                    }
                    //Now add the record
                    db.Languages.Add(language);
                    db.SaveChanges();
                    status = true;

                    SelectNewLanguage(language);
                    _languages = null;
                }                
            }
            catch (Exception exp)
            {
                //TODO: Add to APP Insights
                throw exp;
            }
            return status;                        
        }

        public async Task<bool> DeleteLanguageAsync(Language language)
        {
            bool status = false;
            try
            {
                using (var db = new SQLiteDbContext())
                {
                    if (db.Languages.Any(l => l.LanguageName.Equals(language.LanguageName)))
                    {
                        db.Languages.Remove(language);
                        await db.SaveChangesAsync();
                        status = true;
                    }
                }                
            }
            catch (Exception exp)
            {
                //TODO: Add to APP Insights
                throw exp;
            }
            return status;
        }

        public async Task<ObservableCollection<Language>> GetLanguagesAsync()
        {
            _languages = new ObservableCollection<Language>();
            try
            {
                using (var db = new SQLiteDbContext())
                {
                    _languages = new ObservableCollection<Language>(await db.Languages.ToListAsync());
                }                
            }
            catch (Exception exp)
            {
                //TODO: Add to APP Insights
                throw exp;
            }
            return _languages;
        }

        public async Task<Language> GetSelectedLanguageAsync()
        {
            Language selectedLanguage = null;
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;

                if (localSettings.Keys.Contains(SelectedLanguageKey))
                {
                    var selectedLanguageName = localSettings[SelectedLanguageKey].ToString();
                    if (_languages == null || _languages.Count() == 0)
                        await GetLanguagesAsync();
                    selectedLanguage = _languages.FirstOrDefault(l => l.LanguageName.Equals(selectedLanguageName));
                }
                else
                {
                    await CopyLocalDatabaseToLocalStateFolderAsync();
                    if(_languages == null || _languages.Count() == 0)
                        await GetLanguagesAsync();
                    selectedLanguage = _languages.FirstOrDefault(l => l.LanguageName.Equals("English"));
                    if (selectedLanguage != null)
                        localSettings[SelectedLanguageKey] = selectedLanguage.LanguageName;
                    else
                        throw new Exception("English language does not exits");
                }
            }
            catch(Exception exp)
            {
                //TODO: Add AppInsights
                throw exp;
            }
            return selectedLanguage;            
        }

        public bool SelectNewLanguage(Language language)
        {
            bool status = false;
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
                localSettings[SelectedLanguageKey] = language.LanguageName;
            }
            catch (Exception exp)
            {
                //TODO: Add AppInsights
                throw exp;
            }
            return status;
        }

        private async Task CopyLocalDatabaseToLocalStateFolderAsync()
        {
            try
            {
                var sourceFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var destnationFolder = ApplicationData.Current.LocalFolder;
                await destnationFolder.CreateFolderAsync(App.LocalDbFolderName,CreationCollisionOption.ReplaceExisting);
                File.Copy(Path.Combine(sourceFolder.Path, App.LocalDbFolderName, App.LocalDbName), Path.Combine(destnationFolder.Path, App.LocalDbFolderName, App.LocalDbName),true);

                await destnationFolder.CreateFolderAsync(App.AlphabetsByLanguageFolderName, CreationCollisionOption.ReplaceExisting);

                var predefinedLanguageFiles = await (await sourceFolder.GetFolderAsync(App.AlphabetsByLanguageFolderName)).GetFilesAsync();

                foreach(var predefinedLanguageFile in predefinedLanguageFiles)
                {
                    File.Copy(Path.Combine(sourceFolder.Path, App.AlphabetsByLanguageFolderName, predefinedLanguageFile.Name), Path.Combine(destnationFolder.Path, App.AlphabetsByLanguageFolderName, predefinedLanguageFile.Name), true);
                }
            }
            catch(Exception exp)
            {
                //TODO  Add AppInsights
                throw exp;
            }
        }
    }
}
