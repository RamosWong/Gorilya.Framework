using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Response.Model
{
    public class ModelConstants
    {
        public const string GenericResource = "Gorilya.Framework.Core.Response.Resources.GenericMessages";

        /// <summary>
        /// Keys that are defined in the GenericMessages Resource.
        /// </summary>
        public class GenericMessages
        {
            public class Success
            {
                /// <summary>
                /// The defined key for Generic Success Messages.
                /// </summary>
                public const string GenericSuccessMessage = "S_GenericMessage";
            }

            public class Failed
            {
                /// <summary>
                /// The defined key for Generic Failed Messages.
                /// </summary>
                public const string GenericFailedMessage = "F_GenericMessage";

                /// <summary>
                /// The defined key if the calling application did not register a Resource.
                /// </summary>
                public const string UnregisteredResource = "F_RHU_001";

                /// <summary>
                /// The defined key if the Handler is unable to locate the registered Resource.
                /// </summary>
                public const string UnlocatableResource = "F_RHU_002";
            }
        }
    }
}
