using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace InkingAlphabets.Model
{
    public class AlphabetsDataService : IAlphabetsDataService
    {
        public async Task<IEnumerable<Alphabet>> GetAlphabets(string language)
        {         
            List<Alphabet> alphabets = new List<Alphabet>();
            
            // same as (ms-appx:///MyFolder/MyFile.txt)            
            var localFolder = ApplicationData.Current.LocalFolder;
            // acquire file
            //var _File = await localFolder.CreateFileAsync("MyFile.txt", CreationCollisionOption.OpenIfExists);
            var alphabetsByLanguageFolderName = await localFolder.GetFolderAsync(App.AlphabetsByLanguageFolderName);

            var _languageFile = await alphabetsByLanguageFolderName.GetFileAsync(string.Concat(language,".txt"));

            // read content
            IList<string> alphabetLines = await FileIO.ReadLinesAsync(_languageFile,UnicodeEncoding.Utf8);
            if(alphabetLines.Count ==0)
            {
                throw new Exception("No Alphabets");
                // write content
                //var _WriteThis = "సా\nయి\nరా\nమ్";
                //var _WriteThis = "Om Sairam";                
                //var _WriteThis = "ABCDEFGH";
                //await Windows.Storage.FileIO.WriteTextAsync(_File, _WriteThis);

                // read content
                //alphabetLines = await Windows.Storage.FileIO.ReadLinesAsync(_File, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            foreach(string alphabetLine in alphabetLines)
            {
                alphabets.Add(new Alphabet()
                {
                    AlphabetCharacter = alphabetLine,
                    InkStream = new InMemoryRandomAccessStream()
                });
            }

            return alphabets;
        }

        public Task<DataItem> GetData()
        {
            // Use this to connect to the actual data service

            // Simulate by returning a DataItem
            var item = new DataItem("Inking Alphabets: English");
            return Task.FromResult(item);
        }

        public async Task<bool> SaveAlphabetsAsync(string language, string alphabetsString)
        {
            try
            { 
                var localFolder = ApplicationData.Current.LocalFolder;
                var alphabetsByLanguageFolderName = await localFolder.GetFolderAsync(App.AlphabetsByLanguageFolderName);
                var _File = await alphabetsByLanguageFolderName.CreateFileAsync(language + ".txt", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(_File, alphabetsString);
            }
            catch(Exception exp)
            {
                //TODO Write to logs 
                throw exp;
            }
            return true;
        }

        public async Task<bool> DeleteAlphabetsAsync(string language)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var alphabetsByLanguageFolderName = await localFolder.GetFolderAsync(App.AlphabetsByLanguageFolderName);
               var alphabetFileTobeDeleted = await  alphabetsByLanguageFolderName.GetFileAsync(language + ".txt");
                await alphabetFileTobeDeleted.DeleteAsync(); 
            }
            catch (Exception exp)
            {
                //TODO Write to logs 
                throw exp;
            }
            return true;
        }
    }
}