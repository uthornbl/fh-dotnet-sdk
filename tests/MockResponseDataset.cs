using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Hash;
using FHSDK.Sync;

namespace tests
{
    public class MockResponseDataset<T> : FHSyncDataset<T> where T : IFHSyncModel
    {
        public readonly FHResponse AppliedCreateResponse = new FHResponse(HttpStatusCode.OK,
            @"{
	            ""hash"": ""084540e99a0179151841b2548daa6d98d5a81d89"",
	            ""updates"": {
		            ""hashes"": {
			            ""072f620bdd0c5285d60b1be3dc0600f07585d21c"": {
				            ""cuid"": ""922f3ef3e8c0ad617e69eecb70910917"",
				            ""type"": ""applied"",
				            ""action"": ""create"",
				            ""hash"": ""072f620bdd0c5285d60b1be3dc0600f07585d21c"",
				            ""uid"": ""5630e184e48dca6421000002"",
				            ""msg"": ""''""
			            }
		            },
		            ""applied"": {
			            ""072f620bdd0c5285d60b1be3dc0600f07585d21c"": {
				            ""cuid"": ""922f3ef3e8c0ad617e69eecb70910917"",
				            ""type"": ""applied"",
				            ""action"": ""create"",
				            ""hash"": ""072f620bdd0c5285d60b1be3dc0600f07585d21c"",
				            ""uid"": ""072f620bdd0c5285d60b1be3dc0600f07585d21c"",
				            ""msg"": ""''""
			            }
		            }
	            }
            }");

        public MockResponseDataset(string datasetId)
        {
            DatasetId = datasetId;
            SyncConfig = new FHSyncConfig {SyncCloud = FHSyncConfig.SyncCloudType.Mbbas};
            MetaData = new FHSyncMetaData();
            QueryParams = new Dictionary<string, string>();
            UidMapping = new Dictionary<string, string>();
            dataRecords = new InMemoryDataStore<FHSyncDataRecord<T>>
            {
                PersistPath = GetPersistFilePathForDataset(SyncConfig, datasetId, DATA_PERSIST_FILE_NAME)
            };
            pendingRecords = new InMemoryDataStore<FHSyncPendingRecord<T>>
            {
                PersistPath = GetPersistFilePathForDataset(SyncConfig, datasetId, PENDING_DATA_PERSIST_FILE_NAME)
            };

            ServiceFinder.RegisterType<IHashService, TestHasher>();
            Save();
        }

        public object SyncParams { get; set; }
        public FHResponse MockResponse { get; set; }

        protected override Task<FHResponse> DoCloudCall(object syncParams)
        {
            SyncParams = syncParams;
            return Task.Factory.StartNew(() => MockResponse);
        }

        public class TestHasher : IHashService
        {
            public string GenerateSha1Hash(string str)
            {
                return "072f620bdd0c5285d60b1be3dc0600f07585d21c";
            }
        }
    }
}