using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Cache.Model
{
    internal class ApiCache
    {
        // Reminder: Update StructureId in CacheConstants if anything here is modified.

        /// <summary>
        /// The Data that is being and its Associated Information.
        /// </summary>
        public ApiCacheData CacheData { get; set; }        

        /// <summary>
        /// Contains the Meta Information of the Cache File.
        /// </summary>
        public ApiCacheMeta Meta { get; set; }

        /// <summary>
        /// Contains the History Information of the Cache File.
        /// </summary>
        public ApiCacheDataHistory History { get; set; }
    }        
}
