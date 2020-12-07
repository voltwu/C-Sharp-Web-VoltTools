using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VoltTools.Models
{
    public interface IDatabase
    {
        public Page GetPage(Expression<Func<Page, bool>> filter);
        public List<Page> GetAllPages(Expression<Func<Page, bool>> filter);
        public Task<List<Page>> GetAllPagesAsync(Expression<Func<Page, bool>> filter);
        public Task<Page> GetPageAsync(Expression<Func<Page, bool>> filter);
    }
    public class Mongo : IDatabase
    {

        public String connectionString = String.Empty;
        public String DatabaseName = "volttools";
        public String pageCollectionName = "pages";
        private IMongoClient mongoClient = null;
        private IMongoDatabase mongoDb = null;
        public Mongo()
        {
            connectionString = Environment.GetEnvironmentVariable("connectionstring");
            mongoClient = new MongoClient(connectionString);
            mongoDb = mongoClient.GetDatabase(DatabaseName);
        }
        public virtual Page GetPage(Expression<Func<Page, bool>> filter)
        {
            return GetAllPages(filter).FirstOrDefault<Page>();
        }
        public virtual List<Page> GetAllPages(Expression<Func<Page, bool>> filter)
        {
            return mongoDb.GetCollection<Page>(pageCollectionName).Find(filter).ToList();
        }
        public virtual async Task<List<Page>> GetAllPagesAsync(Expression<Func<Page, bool>> filter)
        {
            return await mongoDb.GetCollection<Page>(pageCollectionName).Find(filter).ToListAsync();
        }
        public virtual async Task<Page> GetPageAsync(Expression<Func<Page, bool>> filter)
        {
            return (await GetAllPagesAsync(filter)).FirstOrDefault<Page>();
        }
    }
}
