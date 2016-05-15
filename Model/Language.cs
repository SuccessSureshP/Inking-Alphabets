using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InkingAlphabets.Model
{
    [DataContract]
    public class Language
    {
        [Key]
        [DataMember]
        public string LanguageName { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public int AlphabetsCount { get; set; }
        [DataMember]
        public int Score { get; set; }

    }
}
