using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Response.Model
{
    public enum Status
    {
        /// <summary>
        /// Indicates that the Process was Completed Successfully.
        /// </summary>
        SUCCESS,

        /// <summary>
        /// Indicates that the Process encountered an Error/Exception.
        /// </summary>
        FAILED,
    }

    public class ApiResponse
    {
        /// <summary>
        /// The Class and Function making the call.
        /// </summary>
        public string Caller { get; set; }

        /// <summary>
        /// Indicates the Status of the Response.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Provides a Code for the Response.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Contains the Detailed Description of the Response.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The object that the user wants to retrieve.
        /// </summary>
        public object Payload { get; set; }
    }
}
