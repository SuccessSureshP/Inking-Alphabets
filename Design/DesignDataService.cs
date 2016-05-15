using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InkingAlphabets.Model;

namespace InkingAlphabets.Design
{
    public class DesignAlphabetsDataService : IAlphabetsDataService
    {
        public Task<DataItem> GetData()
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            return Task.FromResult(item);
        }

        public Task<bool> SaveAlphabetsAsync(string alphabetsString, string language)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Alphabet>> IAlphabetsDataService.GetAlphabets(string language)
        {
            throw new NotImplementedException();
        }

        Task<DataItem> IAlphabetsDataService.GetData()
        {
            throw new NotImplementedException();
        }
    }
}