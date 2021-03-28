using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CD.Models
{
    public interface IDatabase
    {
        public Page GetPage(Expression<Func<Page, bool>> filter);
        public List<Page> GetAllPages(Expression<Func<Page, bool>> filter);
        public Task<CalendarConfiguration> GetCalendarConfiguration();
        public Task<calendarReminder> GetACalendarReminderAsync();
        public Task<UpdateResult> UpdateACalendarReminderAsync(ObjectId oid, DateTime last_executetime);
        public Task<List<Page>> GetAllPagesAsync(Expression<Func<Page, bool>> filter);
        public Task<Page> GetPageAsync(Expression<Func<Page, bool>> filter);
        public Task<List<Shortlink>> GetAllLinksAsync(Expression<Func<Shortlink,bool>> filter);
        public Task<Shortlink> GetLinkAsync(Expression<Func<Shortlink, bool>> filter);
        public Task InsertOrReplaceShortLinkAsync(Shortlink link);
        public Task<ShortLinkConfiguration> GetShortLinkConfiguration();
        public Task<UpdateResult> UpdateShortLinkConfiguration(ShortLinkConfiguration configuration);
        public Task<EmailConfiguration> GetEmailConfiguration();
        public Task<Boolean> IsIfHasAvailableCalendarReminderTask();
        public void InsertAErrorLog(ErrorLog errorLog);
    }
    public class Mongo : IDatabase
    {

        public String connectionString = String.Empty;
        public String DatabaseName = "volttools";
        public String pageCollectionName = "pages";
        public String shortLinkCollectionName = "shortlink";
        public String configurationCollectionName = "configuration";
        public String calendarReminderCollectionName = "calendarReminder";
        public String errorlogsCollectionName = "errorlogs";
        private IMongoClient mongoClient = null;
        private IMongoDatabase mongoDb = null;
        public Mongo()
        {
            //connectionString = Environment.GetEnvironmentVariable("connectionstring");
            connectionString = "mongodb+srv://volttoolsadmin:voltadmin<secret98.a@volttools.uonol.mongodb.net/volttools?retryWrites=true&w=majority";
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
        public virtual async Task<List<Shortlink>> GetAllLinksAsync(Expression<Func<Shortlink, bool>> filter)
        {
            return await mongoDb.GetCollection<Shortlink>(shortLinkCollectionName).Find(filter).ToListAsync();
        }
        public virtual async Task<Shortlink> GetLinkAsync(Expression<Func<Shortlink, bool>> filter) {
            return (await GetAllLinksAsync(filter)).FirstOrDefault<Shortlink>();
        }
        public virtual async Task InsertOrReplaceShortLinkAsync(Shortlink link) {
            await mongoDb.GetCollection<Shortlink>(shortLinkCollectionName).ReplaceOneAsync(
                s=>s.short_url.Equals(link.short_url)
                , link
                , new ReplaceOptions { IsUpsert = true });
        }

        public async Task<ShortLinkConfiguration> GetShortLinkConfiguration()
        {
            var res = await mongoDb.GetCollection<ShortLinkConfiguration>(configurationCollectionName).FindAsync(filter=>  
                filter.Id.Equals(new MongoDB.Bson.ObjectId("5fd48511626f4716ad699c9c"))
            );
            return await res.FirstOrDefaultAsync();
        }

        public async Task<UpdateResult> UpdateShortLinkConfiguration(ShortLinkConfiguration configuration)
        {
            //var doc = new BsonDocument() { .... }
            UpdateDefinition<ShortLinkConfiguration> update = Builders<ShortLinkConfiguration>.Update
                .Set(x => x.IndexTracker, configuration.SerializeIndexTracker())
                .Set(x => x.version, configuration.version + 1)
                .Set(x => x.lastStageTime, configuration.lastStageTime);

            return await mongoDb.GetCollection<ShortLinkConfiguration>(configurationCollectionName)
                .UpdateOneAsync(filter=>  filter.Id.Equals(configuration.Id)
                        && filter.version == configuration.version
                    ,update);
        }

        public async Task<EmailConfiguration> GetEmailConfiguration()
        {
            var res = await mongoDb.GetCollection<EmailConfiguration>(configurationCollectionName).FindAsync(filter =>
                filter.Id.Equals(new MongoDB.Bson.ObjectId("5fd4ec7d10b859e2f06c3402"))
            );
            return await res.FirstOrDefaultAsync();
        }
        public async Task<CalendarConfiguration> GetCalendarConfiguration()
        {
            var res = await mongoDb.GetCollection<CalendarConfiguration>(configurationCollectionName).FindAsync(filter =>
                filter.Id.Equals(new MongoDB.Bson.ObjectId("5fd57844547a6eec59b6e040"))
            );
            return await res.FirstOrDefaultAsync();
        }

        public async Task<bool> IsIfHasAvailableCalendarReminderTask()
        {
            return await mongoDb.GetCollection<calendarReminder>(calendarReminderCollectionName).Find(filter =>
                filter.is_valid && filter.last_executetime <= DateTime.UtcNow.AddDays(-1)).AnyAsync() ;
        }

        public async Task<calendarReminder> GetACalendarReminderAsync()
        {
           return await mongoDb.GetCollection<calendarReminder>(calendarReminderCollectionName).Find(filter =>
               filter.is_valid && filter.last_executetime <= DateTime.UtcNow.AddDays(-1)).FirstOrDefaultAsync();
        }

        public async Task<UpdateResult> UpdateACalendarReminderAsync(ObjectId oid, DateTime last_executetime)
        {
            UpdateDefinition<calendarReminder> update = Builders<calendarReminder>.Update
                .Set(x => x.last_executetime , last_executetime);

            return await mongoDb.GetCollection<calendarReminder>(calendarReminderCollectionName)
                .UpdateOneAsync(filter => filter.Id.Equals(oid)
                    && filter.last_executetime <= DateTime.UtcNow.AddDays(1)
                    && filter.is_valid
                    , update);
        }

        public void InsertAErrorLog(ErrorLog errorLog)
        {
            mongoDb.GetCollection<ErrorLog>(errorlogsCollectionName).InsertOne(errorLog);
        }
    }
}
