﻿using System;
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
            }
            foreach (string alphabetLine in alphabetLines)
            {
                //alphabets.Add(new Alphabet()
                //{
                //    AlphabetCharacter = alphabetLine,
                //    InkStream = new InMemoryRandomAccessStream()
                //});

                var alphabets_list =  alphabetLine.Split(new string[] { " " }, StringSplitOptions.None);
                foreach(var alphabet in alphabets_list)
                {
                    if (!string.IsNullOrEmpty(alphabet))
                    {
                        alphabets.Add(new Alphabet()
                        {
                            AlphabetCharacter = alphabet,
                            InkStream = new InMemoryRandomAccessStream()
                        });
                    }
                }

                //var textElementEnumerator = System.Globalization.StringInfo.GetTextElementEnumerator(alphabetLine);
                //while (textElementEnumerator.MoveNext())
                //{
                //    alphabets.Add(new Alphabet()
                //    {
                //        AlphabetCharacter = textElementEnumerator.GetTextElement(),
                //        InkStream = new InMemoryRandomAccessStream()
                //    });
                //}
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
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"SaveAlphabetsAsync Failed with Exception:{exp.Message}");
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"language Selected :{language}");
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"AlphabetsString :{alphabetsString}");

                return false;
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
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"DeleteAlphabetsAsync Failed with Exception:{exp.Message}");
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"language Selected :{language}");
                return false;
            }
            return true;
        }
    }
}