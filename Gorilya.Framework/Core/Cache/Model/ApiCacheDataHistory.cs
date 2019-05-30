using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gorilya.Framework.Core.Cache.Model
{
    internal class ApiCacheDataHistory
    {
        // Reminder: Update StructureId in CacheConstants if anything here is modified.
        
        /// <summary>
        /// The Max Number of Items allowed in the HistoryStack
        /// </summary>
        public int HistoryMax { get; set; }

        /// <summary>
        /// History Stack of Previously Cached Data
        /// </summary>
        [JsonConverter(typeof(DropoutStackConverter))]
        public DropoutStack<ApiCacheData> HistoryStack { get; set; }
    }

    /// <summary>
    /// Converts the Custom DropoutStack data structure into something that is Serializable.
    /// </summary>
    internal class DropoutStackConverter : JsonConverter<DropoutStack<ApiCacheData>>
    {
        public override void WriteJson(JsonWriter writer, DropoutStack<ApiCacheData> value, JsonSerializer serializer)
        {
            DropoutStack<ApiCacheData> cachedData = (DropoutStack<ApiCacheData>)value;
            List<ApiCacheData> cacheList = null;

            if (cachedData != null)
            {
                // retrieve the List from the DropoutStack
                cacheList = cachedData.ToList;
            }

            // serialise the DropoutStack data using the list
            serializer.Serialize(writer, cacheList);
        }

        public override DropoutStack<ApiCacheData> ReadJson(JsonReader reader, Type objectType, DropoutStack<ApiCacheData> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            DropoutStack<ApiCacheData> stackData = new DropoutStack<ApiCacheData>();

            if (reader.TokenType != JsonToken.Null)
            {
                // the DropoutStack is serialised as a list, so ensure that it is an array type
                if (reader.TokenType == JsonToken.StartArray)
                {
                    JToken token = JToken.Load(reader);

                    // map the JSON List to a C# list of objects
                    List<ApiCacheData> items = token.ToObject<List<ApiCacheData>>();
                    // initialise the List into a DropoutStack
                    stackData = new DropoutStack<ApiCacheData>(items);
                }
            }

            return stackData;
        }
    }
}
