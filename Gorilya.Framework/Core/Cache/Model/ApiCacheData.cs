using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Cache.Model
{
    internal class ApiCacheData
    {
        // Reminder: Update StructureId in CacheConstants if anything here is modified.
        
        /// <summary>
        /// Auto-Generated Identifier to uniquely identify the Cache Data.
        /// </summary>
        public string CacheDataId { get; set; }

        /// <summary>
        /// DateTime of when the data was first Created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// DateTime of when the data was Modified.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }        

        /// <summary>
        /// The actual Data that is being Cached.
        /// </summary>
        public object Payload { get; set; }        
    }
}
