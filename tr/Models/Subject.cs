using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tr.Models
{
    public class Subject
    {
        public long Id { get; set; }
        public string Sub { get; set; }
        public int method { get; set; }
        public long Created { get; set; }
        public long Updated { get; set; }
        public long Expiry { get; set; }
        public string Key { get; set; }
        [MaxLength(20)]
        public string Status { get; set; }
        public string Purpose { get; set; }
    }
    //https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio
}
