using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VoltTools.Models
{
    public class Shortlink
    {
        [BsonIgnoreIfDefault]
        public ObjectId id { set; get; }
        public String target_url { set; get; }
        public String short_url { set; get; }
        public DateTime expire { set; get; }
    }
    public class ShortLinkConfiguration {
        public MongoDB.Bson.ObjectId Id { set; get; }
        public int version { set; get; }
        public int maxAgeMinutes { set; get; }
        public int defaultUrlLenth { set; get; }
        public String IndexTracker { set; get;}
        public DateTime lastStageTime { set;get;}
        public String description { set; get; }
        public List<Int32> DeserializeIndexTracker() {
            return JsonConvert.DeserializeObject<List<Int32>>(IndexTracker.Trim('\"'));
        }
        public String SerializeIndexTracker() {
            return JsonConvert.SerializeObject(IndexTracker);
        }
        public void SetIndexTracker(List<Int32> indexTracker) {
            this.IndexTracker = JsonConvert.SerializeObject(indexTracker);
        }
    }
}
