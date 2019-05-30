using Gorilya.Framework.Core.Cache;
using Gorilya.Framework.Core.Cache.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.ApiModel;

namespace Tester
{
    public class SampleCache
    {
        public void StartTest()
        {
            var testData1 = InitTestData1();
            var testData2 = InitTestData2();
            var testData3 = InitTestData3();
            
            Console.WriteLine("Running Test 1");
            CacheService<ApiCacheTest> cacheService1 = new CacheService<ApiCacheTest>("test.cache", CacheConstants.Behaviour.HISTORY);
            var test1 = cacheService1.SetMaxHistoryStack(5);
            Console.WriteLine(test1.Message);

            var test2 = cacheService1.StoreCache(testData1);
            Console.WriteLine(test2.Message);

            Console.WriteLine("Running Test 2");
            CacheService<ApiCacheTest> cacheService2 = new CacheService<ApiCacheTest>("test.cache", CacheConstants.Behaviour.HISTORY);
            var test3 = cacheService2.StoreCache(testData2);
            Console.WriteLine(test3.Message);

            Console.WriteLine("Running Test 3");
            CacheService<ApiCacheTest> cacheService3 = new CacheService<ApiCacheTest>("test.cache", CacheConstants.Behaviour.HISTORY);
            var test4 = cacheService3.StoreCache(testData3);
            Console.WriteLine(test4.Message);
            
            var test5 = cacheService3.SetMaxHistoryStack(2);
            Console.WriteLine(test5.Message);
            //cacheService3.ClearHistoryStack();
            var test6 = cacheService3.SaveChanges();
            Console.WriteLine(test6.Message);


            var test7 = cacheService3.RetrieveCache();
            var test7Payload = (ApiCacheTest)test7.Payload;

            Console.WriteLine("{0}'s Favourite Number is {1}.", test7Payload.Name, test7Payload.FavouriteNumber);
            Console.WriteLine("{0}'s secret is {1}.", test7Payload.Name, test7Payload.Secrets.First().Secret);

            Console.WriteLine("Enter");
            Console.ReadLine();
        }

        public ApiCacheTest InitTestData1()
        {
            var testData = new ApiCacheTest()
            {
                Name = "Ron",
                FavouriteNumber = 17
            };

            testData.Secrets = new List<ApiInternalCacheTest>();
            testData.Secrets.Add(new ApiInternalCacheTest()
            {
                SecretId = "Secret1",
                Secret = "I like turtles."
            });

            return testData;
        }

        public ApiCacheTest InitTestData2()
        {
            var testData = new ApiCacheTest()
            {
                Name = "Robert",
                FavouriteNumber = 50
            };

            testData.Secrets = new List<ApiInternalCacheTest>();
            testData.Secrets.Add(new ApiInternalCacheTest()
            {
                SecretId = "Secret2",
                Secret = "I like tortoises more!"
            });

            return testData;
        }

        public ApiCacheTest InitTestData3()
        {
            var testData = new ApiCacheTest()
            {
                Name = "LeBron",
                FavouriteNumber = 23
            };

            testData.Secrets = new List<ApiInternalCacheTest>();
            testData.Secrets.Add(new ApiInternalCacheTest()
            {
                SecretId = "Secret3",
                Secret = "I still like the Cavs"
            });

            return testData;
        }
    }
}
