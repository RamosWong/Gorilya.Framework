using System;
using System.Collections.Generic;
using System.Text;
using static Gorilya.Framework.Core.Cache.Model.CacheConstants;

namespace Gorilya.Framework.Core.Cache.Model
{
    internal class ApiCacheService
    {
        /// <summary>
        /// The Actual Cache Data.
        /// </summary>
        public ApiCache Cache { get; set; }

        /// <summary>
        /// The Behaviour that the CacheService should use.
        /// </summary>
        public Behaviour CacheBehaviour { get; set; }

        /// <summary>
        /// The Full File Path to the Cache File.
        /// </summary>
        public string CacheFilePath { get; set; }
        /// <summary>
        /// The FileName of the Cache File (with Extension).
        /// </summary>
        public string CacheFileName { get; set; }

        /// <summary>
        /// The Full Folder Path that contains the Cache File(s).
        /// </summary>
        public string CacheFolderPath { get; set; }
        /// <summary>
        /// The Folder Name that contains the Cache File(s).
        /// </summary>
        public string CacheFolderName { get; set; }

        /// <summary>
        /// The Max Number of Items allowed in the History Stack.
        /// </summary>
        public int? CacheHistoryMax { get; set; }
    }
}
