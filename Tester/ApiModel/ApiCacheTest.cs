using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.ApiModel
{
    public class ApiCacheTest
    {
        public string Name { get; set; }
        
        public int FavouriteNumber { get; set; }  

        public List<ApiInternalCacheTest> Secrets { get; set; } 
    }
}
