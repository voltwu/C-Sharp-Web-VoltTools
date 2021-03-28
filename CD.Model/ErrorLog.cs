using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CD.Models
{
    public class ErrorLog
    {
        public MongoDB.Bson.ObjectId Id { set { Id = value; } get { return MongoDB.Bson.ObjectId.GenerateNewId(); } }
        public DateTime occurtime { set; get; }
        public String rawurl { set; get; }
        public String url { set; get; }
        public string ip { set; get; }
        public String message { set; get; }
        public String param { set; get; }
        public String body { set; get; }
        public String stackTrace { set; get; }
    }
}
