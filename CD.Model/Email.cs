using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CD.Models
{
    public class Email
    {
        public String sendername { set; get; }
        public String body { set; get; }
        public String subject { set; get; }
        public String address { set; get; }
        public List<MailAddress> MailAddresses {
            get {
                List<String> strs = JsonConvert.DeserializeObject<List<String>>(address);
                List<MailAddress> md = new List<MailAddress>();
                foreach (String ad in strs)
                    md.Add(new MailAddress(ad));
                return md;
            }
        }
    }
    public class EmailConfiguration { 
        public ObjectId Id { set; get; }
        public int liveSeconds { set; get; }
        public string salt { set; get; }
        public string description { set; get; }
    }
}
