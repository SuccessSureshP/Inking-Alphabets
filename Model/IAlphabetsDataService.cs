using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InkingAlphabets.Model
{
    public interface IAlphabetsDataService
    {
        Task<DataItem> GetData();

        Task<IEnumerable<Alphabet>> GetAlphabets(string language);

        Task<bool> SaveAlphabetsAsync(string language, string alphabetsString);
    }
}