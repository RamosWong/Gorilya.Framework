using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Cache.Model
{
    public class CacheConstants
    {
        /// <summary>
        /// Enumeration for the Caching Behaviour that the Caching Service should use.
        /// </summary>
        public enum Behaviour
        {
            /// <summary>
            /// If a cache with the same filename already exists, the cache service overwrites everything in the file.
            /// </summary>
            OVERWRITE,            

            /// <summary>
            /// If a cache with the same filename already exists and its Modified On date is older than the current date, archive that file with a timestamp. 
            /// However, if the Modified On date is equal to the current date, do an OVERWRITE behaviour.
            /// </summary>
            ARCHIVE,

            /// <summary>
            /// If a cache with the same filename already exists, move the existing payload over to the History then add the new payload.
            /// </summary>
            HISTORY,
        }

        /// <summary>
        /// Meta Information for the Cache File to use.
        /// </summary>
        public class Meta
        {
            /// <summary>
            /// A GUID of the Structure that is being used.
            /// </summary>
            public const string StructureId = "22B3812F-06E8-4301-9283-73A5DE3EAF99";
        }

        /// <summary>
        /// Default values that are used when the user does not explicitly define them.
        /// </summary>
        public class Defaults
        {
            /// <summary>
            /// The Default Folder Name that will contain the Cache File(s).
            /// </summary>
            public const string FolderName = "Cache";

            /// <summary>
            /// The Default Caching Behaviour that the Service should use if not explicitly specified.
            /// </summary>
            public const Behaviour Behaviour = CacheConstants.Behaviour.ARCHIVE;

            /// <summary>
            /// The Default Maximum Capacity the DropoutStack data structure should hold.
            /// </summary>
            public const int DropoutStackMaxCapacity = 10;
        }

        public class Codes
        {
            public class Success
            {
                public const string GenericSuccess = "S_GenericMessage";
                public const string CreationSuccess = "S_CCS_001";
                public const string HistoryStackSizeUpdated = "S_CCS_002";
                public const string NewCacheSuccess = "S_CCS_003";
                public const string DeserialisationSuccess = "S_CCS_004";
            }

            public class Failed
            {
                public const string GenericFailed = "F_GenericMessage";
                public const string CreationFailed = "F_CCS_001";
                public const string SerialisationFailed = "F_CCS_002";
                public const string InvalidBehaviour = "F_CCS_003";
                public const string UnknownStoreError = "F_CCS_004";
                public const string DeserialisationFailed = "F_CCS_005";
                public const string FileArchiveFailed = "F_CCS_006";
            }
        }
    }
}
