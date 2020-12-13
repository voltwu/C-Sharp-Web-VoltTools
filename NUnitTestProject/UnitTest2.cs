using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NUnitTestProject
{
    public class Tests2
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test2()
        {
            List<String> res = new List<String>();
            res.Add("cdcd2013@outlook.com");
            res.Add("cdcd2013@outloo2k.com");
            String ress = JsonConvert.SerializeObject(res);
            Console.WriteLine(ress);
            //List<Int32> col = JsonConvert.DeserializeObject<List<Int32>>(ress);
            //col.ForEach(a => Console.WriteLine(a));
            //var client = new MongoClient("mongodb+srv://volttoolsadmin:voltadmin<secret98.a@volttools.uonol.mongodb.net/volttools?retryWrites=true&w=majority");
            //var database = client.GetDatabase("volttools");
            ////var collections = database.GetCollection<BsonDocument>("pages");
            //var collections = database.GetCollection<Page>("pages");
            ////collections.Find();
            ////var a = collections.ToJson();
            ////var header = collections.Find(x => x._is_nav_show).ToList<Page>();
            ////Assert.AreEqual(header.Count, 5);
            //var query = collections.Find(x => true).ToList();

            Assert.Pass();
        }
        public class Page
        {
            public int id { set; get; }
            public bool isnavshow { set; get; }
            public string link { set; get; }
            public int order { set; get; }
            public int pid { set; get; }
            public string contents { set; get; }
            public string title { set; get; }
        }
    }
}