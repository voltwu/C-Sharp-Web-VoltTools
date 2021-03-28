using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace NUnitTestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var client = new MongoClient("mongodb+srv://volttoolsadmin:voltadmin<secret98.a@volttools.uonol.mongodb.net/volttools?retryWrites=true&w=majority");
            var database = client.GetDatabase("volttools");
            //var collections = database.GetCollection<BsonDocument>("pages");
            var collections = database.GetCollection<Page>("pages");
            //collections.Find();
            //var a = collections.ToJson();
            //var header = collections.Find(x => x._is_nav_show).ToList<Page>();
            //Assert.AreEqual(header.Count, 5);
            var query = collections.Find(x => true).ToList();

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