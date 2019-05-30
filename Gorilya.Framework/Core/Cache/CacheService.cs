using Gorilya.Framework.Core.Cache.Model;
using Gorilya.Framework.Core.Response;
using Gorilya.Framework.Core.Response.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Gorilya.Framework.Core.Cache
{
    public class CacheService<T>
    {        
        private readonly ApiCacheService cacheProperties = new ApiCacheService();

        #region Constructors
        /// <summary>
        /// Service provider to create Cache Files within the application using the predefined Defaults.
        /// </summary>
        /// <param name="cacheDirectoryName">Specifies the Directory Name that will store all the Cache Files.</param>
        /// <param name="cacheFileName">Specifies the File Name of the Cache File (requires extension).</param>
        /// <param name="behaviour">Specifies the Caching Behaviour the application should use.</param>
        public CacheService(string cacheFileName)
        {
            var cacheDirectoryName = CacheConstants.Defaults.FolderName;
            var behaviour = CacheConstants.Defaults.Behaviour;
            InitialiseService(cacheDirectoryName, cacheFileName, behaviour);
        }

        /// <summary>
        /// Service provider to create Cache Files within the application using the predefined Default Directory.
        /// </summary>
        /// <param name="cacheFileName">Specifies the File Name of the Cache File.</param>
        /// <param name="behaviour">Specifies the Caching Behaviour the application should use.</param>
        public CacheService(string cacheFileName, CacheConstants.Behaviour behaviour)
        {
            var cacheDirectoryName = CacheConstants.Defaults.FolderName;
            InitialiseService(cacheDirectoryName, cacheFileName, behaviour);
        }

        /// <summary>
        /// Service provider to create Cache Files within the application.
        /// </summary>
        /// <param name="cacheDirectoryName">Specifies the Directory Name that will store all the Cache Files.</param>
        /// <param name="cacheFileName">Specifies the File Name of the Cache File.</param>
        /// <param name="behaviour">Specifies the Caching Behaviour the application should use.</param>
        public CacheService(string cacheDirectoryName, string cacheFileName, CacheConstants.Behaviour behaviour)
        {
            InitialiseService(cacheDirectoryName, cacheFileName, behaviour);
        }

        /// <summary>
        /// Common Function to initialise the Cache Properties.
        /// </summary>
        /// <param name="cacheDirectoryName">Specifies the Directory Name that will store all the Cache Files.</param>
        /// <param name="cacheFileName">Specifies the File Name of the Cache File.</param>
        /// <param name="behaviour">Specifies the Caching Behaviour the application should use.</param>
        private void InitialiseService(string cacheDirectoryName, string cacheFileName, CacheConstants.Behaviour behaviour)
        {
            ResponseHandler.RegisterResource("Gorilya.Framework.Core.Cache.Resources.CacheMessages");

            InitialiseCachePath(cacheDirectoryName, cacheFileName);
            cacheProperties.CacheBehaviour = behaviour;
            cacheProperties.CacheFileName = cacheFileName;
            cacheProperties.CacheFolderName = cacheDirectoryName;

            var cacheResponse = this.DeserialiseCache();
            if (cacheResponse.Status == Status.FAILED)
            {
                throw new Exception(cacheResponse.Message);
            }

            cacheProperties.Cache = (ApiCache)cacheResponse.Payload;

            if (!this.IsUsingLatestStructure())
            {
                // if the cache is using a different structure, archive it to prevent issues
                this.ArchiveCacheData(cacheProperties.Cache.CacheData, cacheProperties.Cache.Meta.StructureId);

                cacheProperties.Cache = CreateNewCache();
            }

        }
        #endregion        

        #region Public Methods
        /// <summary>
        /// Creates a cache file.
        /// </summary>
        /// <remarks>
        /// This function will automatically save any Property Changes made.
        /// </remarks>
        /// <param name="data">Data to cache.</param>
        public ApiResponse StoreCache(T data)
        {
            if (!string.IsNullOrEmpty(cacheProperties.CacheFilePath))
            {
                var cacheData = JsonConvert.SerializeObject(data, Formatting.Indented);

                if (cacheProperties.CacheBehaviour == CacheConstants.Behaviour.OVERWRITE)
                {
                    var maxItems = cacheProperties.CacheHistoryMax ?? CacheConstants.Defaults.DropoutStackMaxCapacity;

                    var cache = new ApiCache
                    {
                        Meta = new ApiCacheMeta()
                        {
                            StructureId = CacheConstants.Meta.StructureId,
                        },
                        History = new ApiCacheDataHistory()
                        {
                            HistoryMax = maxItems
                        }
                    };

                    var cachedData = CreateCacheData(data);
                    cache.CacheData = cachedData;

                    return this.WriteCacheToFile(cache);
                }
                else if (cacheProperties.CacheBehaviour == CacheConstants.Behaviour.HISTORY)
                {
                    this.AddCacheToHistory(cacheProperties.Cache);
                    var cachedData = CreateCacheData(data);
                    cacheProperties.Cache.CacheData = cachedData;

                    return this.WriteCacheToFile(cacheProperties.Cache);
                }
                else if (cacheProperties.CacheBehaviour == CacheConstants.Behaviour.ARCHIVE)
                {
                    var validForArchiving = this.VerifyArchiveValidity(cacheProperties.Cache);
                    this.ArchiveCacheData(cacheProperties.Cache.CacheData);
                    var cachedData = CreateCacheData(data);
                    cacheProperties.Cache.CacheData = cachedData;

                    return this.WriteCacheToFile(cacheProperties.Cache);
                }
                else
                {
                    return ResponseHandler.BuildResponse("CacheService.StoreCache", CacheConstants.Codes.Failed.InvalidBehaviour, Status.FAILED);
                }
            }
            return ResponseHandler.BuildResponse("CacheService.StoreCache", CacheConstants.Codes.Failed.UnknownStoreError, Status.FAILED);
        }

        /// <summary>
        /// Saves any Property Changes made and updates the Cache Data.
        /// </summary>
        /// <remarks>
        /// Any Property Changes made will not be saved until this method is called.
        /// </remarks>
        public ApiResponse SaveChanges()
        {
            return this.WriteCacheToFile(cacheProperties.Cache);
        }

        /// <summary>
        /// Retrieves Cached Data from file.
        /// </summary>
        /// <returns>Returns the Cached Data.</returns>
        public ApiResponse RetrieveCache()
        {
            ApiCache cache = null;

            var cacheResponse = this.DeserialiseCache();
            if (cacheResponse.Status == Status.FAILED)
            {
                return cacheResponse;
            }

            cache = (ApiCache)cacheResponse.Payload;

            var cacheData = cache.CacheData;

            return ResponseHandler.BuildPayloadResponse((T)cacheData.Payload);
        }
        #endregion

        #region External Utility Methods
        #region History Utilities
        /// <summary>
        /// Sets the Maximum Number of Items allowed in the HistoryStack.
        /// </summary>
        /// <param name="maxItems">Specifies the Desired HistoryStack Limit.</param>
        public ApiResponse SetMaxHistoryStack(int maxItems)
        {
            cacheProperties.CacheHistoryMax = maxItems;

            if (cacheProperties.Cache.History != null)
            {
                cacheProperties.Cache.History.HistoryMax = maxItems;

                if (cacheProperties.Cache.History.HistoryStack != null)
                {
                    cacheProperties.Cache.History.HistoryStack.UpdateMaximumCapacity(maxItems);
                }
            }

            return ResponseHandler.BuildResponse("CacheService.SetMaxHistoryStack", "S_CCS_002", Status.SUCCESS);
        }

        /// <summary>
        /// Reverts the Current Data to the Data from the HistoryStack that matches the GUID.
        /// </summary>
        /// <param name="cacheDataGuid">The GUID of the Data to revert to.</param>
        public void RevertHistory(string cacheDataGuid)
        {            
            // search for the requested cache data
            var searchPredicate = new Func<ApiCacheData, bool>(x => x.CacheDataId == cacheDataGuid);
            ApiCacheData cacheData = cacheProperties.Cache.History.HistoryStack.GetItemFromStack(searchPredicate);

            // add the existing data to the history stack
            var existingData = cacheProperties.Cache.CacheData;
            cacheProperties.Cache.History.HistoryStack.Push(existingData);

            // revert the current data to the requested cache data
            cacheProperties.Cache.CacheData = cacheData;
            cacheProperties.Cache.History.HistoryStack.RemoveItemFromStack(cacheData);
        }

        /// <summary>
        /// Clears the Items in the HistoryStack.
        /// </summary>
        public void ClearHistoryStack()
        {
            if (cacheProperties.Cache.History != null)
            {
                cacheProperties.Cache.History.HistoryStack = null;
            }
        }
        #endregion


        #endregion

        #region Internal Utility Methods
        /// <summary>
        /// Verifies that the Existing Cache file is using the Latest Structure
        /// </summary>
        /// <returns></returns>
        private bool IsUsingLatestStructure()
        {
            var cacheFileStructureId = cacheProperties.Cache.Meta.StructureId;
            
            if (cacheFileStructureId.Equals(CacheConstants.Meta.StructureId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initialises the required Directories and Files for the Cache File(s)
        /// </summary>
        /// <param name="cacheDirectoryName">Specifies the Directory Name that will hold the Cache File</param>
        /// <param name="cacheFileName">Specifies the Cache Filename</param>
        /// <returns>Returns the Full Path to the Cache File</returns>
        protected string InitialiseCachePath(string cacheDirectoryName, string cacheFileName)
        {
            string assemblyPath;

            try
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                assemblyPath = Path.GetDirectoryName(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var cacheDirectoryPath = Path.Combine(assemblyPath, cacheDirectoryName);
            if (!Directory.Exists(cacheDirectoryPath))
            {
                Directory.CreateDirectory(cacheDirectoryPath);
            }

            cacheProperties.CacheFolderPath = cacheDirectoryPath;

            var cacheFilePath = Path.Combine(cacheDirectoryPath, cacheFileName);
            if (!File.Exists(cacheFilePath))
            {
                var file = File.Create(cacheFilePath);
                file.Close();
            }

            cacheProperties.CacheFilePath = cacheFilePath;

            return cacheFilePath;
        }

        /// <summary>
        /// Creates and Initialises a New Cache.
        /// </summary>
        /// <returns>Return the New Cache object.</returns>
        private ApiCache CreateNewCache()
        {
            return new ApiCache()
            {
                History = new ApiCacheDataHistory()
                {
                    HistoryMax = cacheProperties.CacheHistoryMax.Value,
                },
                Meta = new ApiCacheMeta()
                {
                    StructureId = CacheConstants.Meta.StructureId,
                }
            };
        }

        /// <summary>
        /// Creates the Cache Data using the Structure.
        /// </summary>
        /// <typeparam name="T">Type of the Object that will be cached.</typeparam>
        /// <param name="payload">The Object that will be cached.</param>
        /// <returns>Returns the Structure containing the Cache Data.</returns>
        private ApiCacheData CreateCacheData(T payload)
        {
            return new ApiCacheData()
            {
                CacheDataId = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                Payload = payload,
            };
        }

        /// <summary>
        /// Modifies the existing the Cache Data using the Structure.
        /// </summary>
        /// <typeparam name="T">Type of the Object that will be cached.</typeparam>
        /// <param name="payload">The Object that will be cached.</param>
        /// <returns>Returns the Structure containing the Cache Data.</returns>
        private ApiCacheData ModifyCacheData(T payload)
        {
            return new ApiCacheData()
            {
                CacheDataId = Guid.NewGuid().ToString(),
                ModifiedOn = DateTime.Now,
                Payload = payload,
            };
        }

        /// <summary>
        /// Moves the Predecessing Payload to the HistoryStack.
        /// </summary>
        /// <param name="cache">The Existing Cache.</param>
        private void AddCacheToHistory(ApiCache cache)
        {
            var cacheData = cache.CacheData;

            if (cacheData != null)
            {
                if (cache.History.HistoryStack == null)
                {
                    var maxItems = cacheProperties.CacheHistoryMax ?? CacheConstants.Defaults.DropoutStackMaxCapacity;

                    cache.History.HistoryStack = new DropoutStack<ApiCacheData>(maxItems);
                }

                cache.History.HistoryStack.Push(cacheData);
            }
        }

        /// <summary>
        /// Validates whether to the Cache is valid for Archiving
        /// (based on Created/Modified date).
        /// </summary>
        /// <param name="cache">The Cache to Archive.</param>
        private bool VerifyArchiveValidity(ApiCache cache)
        {
            var cacheData = cache.CacheData;
            
            if (cacheData != null)
            {
                if (cacheData.ModifiedOn.HasValue)
                {
                    if (cacheData.ModifiedOn.Value.Date < DateTime.Now.Date)
                    {
                        return true;
                    }
                }
                else
                {
                    if (cacheData.CreatedOn.Date < DateTime.Now.Date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Renames the old Cache File.
        /// </summary>
        /// <param name="cacheData">The Data that is being Cached.</param>
        /// <param name="fileNamePrefix">Special Prefix to add to the FileName</param>
        private void ArchiveCacheData(ApiCacheData cacheData, string fileNamePrefix = null)
        {
            var formattedDate = cacheData.CreatedOn.ToString("yyyyMMdd");
            var fileNameOnly = cacheProperties.CacheFileName.Split('.')[0];
            var fileExtension = cacheProperties.CacheFileName.Split('.')[1];

            var archiveFileName = string.Format("{0}.{1}.{2}", formattedDate, fileNameOnly, fileExtension);

            if (!string.IsNullOrEmpty(fileNamePrefix))
            {
                archiveFileName = string.Format("{0}.{1}", fileNamePrefix, archiveFileName);
            }

            var archiveFilePath = Path.Combine(cacheProperties.CacheFolderPath, archiveFileName);

            try
            {
                File.Move(cacheProperties.CacheFilePath, archiveFilePath);
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// Deserialises the Cache from Storage.
        /// </summary>
        /// <returns>Returns the Deserialised Cache</returns>
        private ApiResponse DeserialiseCache()
        {
            ApiCache result = null;
            ApiResponse response = new ApiResponse();

            var json = File.ReadAllText(cacheProperties.CacheFilePath);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<ApiCache>(json);
                }
                catch (Exception ex)
                {
                    return ResponseHandler.BuildResponse("CacheService.DeserialiseCache",
                        CacheConstants.Codes.Failed.DeserialisationFailed,
                        Status.FAILED);
                }

                var maxItems = cacheProperties.CacheHistoryMax ?? result.History.HistoryMax;

                cacheProperties.CacheHistoryMax = maxItems;
                result.History.HistoryStack.UpdateMaximumCapacity(maxItems);

                result.CacheData.Payload = JsonConvert.DeserializeObject<T>
                    (result.CacheData.Payload.ToString());

                var historyItems = result.History.HistoryStack.ToList;
                foreach (var item in historyItems)
                {
                    var deserialisedItem = JsonConvert.DeserializeObject<T>(item.Payload.ToString());
                    item.Payload = deserialisedItem;
                }

                return ResponseHandler.BuildPayloadResponse(result);
            }
            else
            {
                result = new ApiCache()
                {
                    Meta = new ApiCacheMeta()
                    {
                        StructureId = CacheConstants.Meta.StructureId,
                    },
                    History = new ApiCacheDataHistory()
                    {
                        HistoryMax = CacheConstants.Defaults.DropoutStackMaxCapacity,
                    }
                };

                return ResponseHandler.BuildResponse("CacheService.DeserialiseCache",
                    CacheConstants.Codes.Success.NewCacheSuccess,
                    Status.SUCCESS,
                    result);
            }
        }

        /// <summary>
        /// Writes the Cache to Storage.
        /// </summary>
        /// <param name="cache">The Cache to Serialise and Write to Storage.</param>
        private ApiResponse WriteCacheToFile(ApiCache cache)
        {
            if (cache.History != null && cache.History.HistoryStack != null)
            {
                if (cache.History.HistoryStack.Peek() == null)
                {
                    cache.History = null;
                };
            }

            string serialisedData = string.Empty;

            try
            {
                serialisedData = JsonConvert.SerializeObject(cache, Formatting.Indented);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BuildException("CacheService.WriteCacheToFile", CacheConstants.Codes.Failed.SerialisationFailed, ex);
            }

            try
            {
                File.WriteAllText(cacheProperties.CacheFilePath, serialisedData);
                return ResponseHandler.BuildResponse("CacheService.WriteCacheToFile", CacheConstants.Codes.Success.CreationSuccess, Status.SUCCESS);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BuildException("CacheService.WriteCacheToFile", CacheConstants.Codes.Failed.CreationFailed, ex);
            }
        }
        #endregion
    }
}
