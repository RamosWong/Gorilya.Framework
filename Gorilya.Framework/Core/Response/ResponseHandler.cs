using Gorilya.Framework.Core.Response.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Gorilya.Framework.Core.Response
{
    /// <summary>
    /// Used for Basic Responses.
    /// </summary>
    public class ResponseHandler
    {
        private static ApiResponseHandler responseProperties = new ApiResponseHandler();

        /// <summary>
        /// Registers a Resource File for the Handler to use.
        /// </summary>
        /// <param name="resource">The Custom Resource to use.</param>
        public static void RegisterResource(string resource)
        {
            responseProperties.MessageResource = resource;
        }

        /// <summary>
        /// Builds a Payload Response to return the object to the user.
        /// </summary>
        /// <param name="payload">The Payload to add to the response.</param>
        /// <returns>Returns the response object that was built.</returns>
        public static ApiResponse BuildPayloadResponse(object payload)
        {
            return BuildResponse("", ModelConstants.GenericMessages.Success.GenericSuccessMessage, Status.SUCCESS, payload);
        }

        /// <summary>
        /// Builds a Response based on the Parameters provided.
        /// </summary>
        /// <param name="stackTrace">The name of the class and function that made the call.</param>
        /// <param name="status">Indicates the Status of the Response.</param>
        /// <param name="code">The Code to retrieve the Message from the Resource.</param>
        /// <param name="payload">The Payload to return to the user.</param>
        /// <param name="args">The arguments to subsitute into the Resource Message.</param>
        public static ApiResponse BuildResponse(string stackTrace, string code, Status status, object payload = null, params string[] args)
        {
            var hasResource = IsResourceRegistered();
            var resource = GetResource(responseProperties.MessageResource);

            if (!hasResource)
            {
                return new ApiResponse()
                {
                    Caller = "ResponseHandler.BuildResponse",
                    Status = Status.FAILED,
                    Code = ModelConstants.GenericMessages.Failed.UnregisteredResource,
                    Message = string.Format(resource.GetString(ModelConstants.GenericMessages.Failed.UnregisteredResource), args)
                };
            }

            return CreateResponse(resource, code, stackTrace, status, payload, args);
        }

        /// <summary>
        /// Builds a Response based on the Parameters provided.
        /// </summary>
        /// <param name="stackTrace">The name of the class and function that made the call.</param>
        /// <param name="code">The Code to retrieve the Message from the Resource.</param>
        /// <param name="ex">The Exception that was encountered.</param>
        /// <param name="args">The arguments to subsitute into the Resource Message.</param>
        /// <returns></returns>
        public static ApiResponse BuildException(string stackTrace, string code, Exception ex, params string[] args)
        {
            var hasResource = IsResourceRegistered();
            var resource = GetResource(responseProperties.MessageResource);

            if (!hasResource)
            {
                return new ApiResponse()
                {
                    Caller = "ResponseHandler.BuildResponse",
                    Status = Status.FAILED,
                    Code = ModelConstants.GenericMessages.Failed.UnregisteredResource,
                    Message = string.Format(resource.GetString(ModelConstants.GenericMessages.Failed.UnregisteredResource), args)
                };
            }

            return CreateResponse(resource, code, stackTrace, Status.FAILED, null, args);
        }

        /// <summary>
        /// Validates that the calling application has registered a Resource with the Handler.
        /// </summary>
        /// <returns>Returns true or false.</returns>
        private static bool IsResourceRegistered()
        {
            if (string.IsNullOrEmpty(responseProperties.MessageResource))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Instantiates a ResourceManager instance for the provided Resource.
        /// </summary>
        /// <param name="resource">The resource to required.</param>
        /// <returns>Returns a ResourceManager instance for the provided Resource.</returns>
        private static ResourceManager GetResource(string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                return new ResourceManager(ModelConstants.GenericResource, Assembly.GetExecutingAssembly());
            }
            return new ResourceManager(resource, Assembly.GetExecutingAssembly());
        }

        private static ApiResponse CreateResponse(ResourceManager resource, string code, string stackTrace, Status status, object payload, params string[] args)
        {
            var message = string.Empty;

            try
            {
                message = resource.GetString(code);
            }
            catch (MissingManifestResourceException mre)
            {
                resource = new ResourceManager(ModelConstants.GenericResource, Assembly.GetExecutingAssembly());

                return new ApiResponse()
                {
                    Caller = "ResponseHandler.CreateResponse",
                    Status = Status.FAILED,
                    Code = ModelConstants.GenericMessages.Failed.UnregisteredResource,
                    Message = string.Format(resource.GetString(ModelConstants.GenericMessages.Failed.UnlocatableResource), args)
                };
            }
            catch (Exception ex)
            {
                resource = new ResourceManager(ModelConstants.GenericResource, Assembly.GetExecutingAssembly());

                return new ApiResponse()
                {
                    Caller = "ResponseHandler.CreateResponse",
                    Status = Status.FAILED,
                    Code = "OHSH_T",
                    Message = "Unknown Exception while retrieving Message from Resource."
                };
            }

            message = string.Format(message, args);

            return new ApiResponse()
            {
                Caller = stackTrace,
                Status = status,
                Code = code,
                Message = message,
                Payload = payload,
            };
        }
    }
}
