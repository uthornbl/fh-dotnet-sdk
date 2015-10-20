﻿#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FHSDK.Services;
using FHSDK.Services.Hash;
using FHSDK.Sync;
using Newtonsoft.Json.Linq;
#if WINDOWS_PHONE
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
using TearDown = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute;
using FHSDK.Phone;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else
using NUnit.Framework;
#endif

namespace FHSDKTestShared
{
    //To make sure the hash functions on C# will generate the same value as other platforms
    [TestFixture]
    public class FHSyncUtilsTest
    {
        [SetUp]
        public void SetUp()
        {
            FHClient.Init();
        }

        [Test]
        public void TestStringHash()
        {
            var text = "test";
            var hasher = ServiceFinder.Resolve<IHashService>();
            var nativeHashed = hasher.GenerateSha1Hash(text);
            Debug.WriteLine("native hashed value = {0}", nativeHashed);
            var expected = "a94a8fe5ccb19ba61c4c0873d391e987982fbbd3";
            Debug.WriteLine("got hash value {0}", nativeHashed);
            Assert.IsTrue(expected.Equals(nativeHashed));
        }

        [Test]
        public void TestObjectHash()
        {
            var testObject = new JObject();
            testObject["testKey"] = "Test Data";
            testObject["testBoolKey"] = true;
            testObject["testNumKey"] = 10;
            var arr = new JArray();
            arr.Add("obj1");
            arr.Add("obj2");
            testObject["testArrayKey"] = arr;
            var obj = new JObject();
            obj["obj3key"] = "obj3";
            obj["obj4key"] = "obj4";
            testObject["testDictKey"] = obj;
            var hash = FHSyncUtils.GenerateSHA1Hash(testObject);
            Debug.WriteLine("Got hash value = {0}", hash);
            var expected = "5f4675723d658919ede35fac62fade8c6397df1d";
            Assert.IsTrue(expected.Equals(hash));
        }

        [Test]
        public void TestGenerateHashWithUnderscoreInKey()
        {
            // given
            var data = new JObject();
            data["COMMENTS"] = "";
            data["FHID"] = "2553C7ED-9025-48F9-A346-EBE3E3AF943B";
            data["QUESTION_ID"] = 22;
            data["QUES_VALUE"] = "NO";
            data["VISIT_ID"] = 100220;
            data["TEST1_ttt"] = "test";
            data["TEST11_ttt"] = "test2";

            // when
            var hash = FHSyncUtils.GenerateSHA1Hash(data);

            // then
            Assert.AreEqual("824d6ded431d16fe8f2ab02b0744ca06822a3fff", hash, "Hashvalue generated by dotnet SDK doesn't match JS SDK");
        }
    }
}