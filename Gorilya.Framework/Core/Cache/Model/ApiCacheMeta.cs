using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Cache.Model
{
    /// <summary>
    /// Contains the Meta Information of the Cache File
    /// </summary>
    internal class ApiCacheMeta
    {
        // Reminder: Update StructureId in CacheConstants if anything here is modified.

        /// <summary>
        /// The Structure Id that the Cache is using
        /// </summary>
        public string StructureId { get; set; }        
    }
}
