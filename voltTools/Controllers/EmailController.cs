using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CD.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VoltTools.Models;
using WebApplication1.Controllers;

namespace VoltTools.Controllers
{
    public class EmailController : BaseController
    {
        public async Task<IActionResult> Send([FromServices]IDatabase _database
            ,[FromServices] EmailSender sender
            , Email email,DateTime sdt, String md5v) {
            JObject res = new JObject();
            if (email == null
                || String.IsNullOrEmpty(email.body)
                || String.IsNullOrEmpty(email.address)
                || String.IsNullOrEmpty(email.subject)
                || email.MailAddresses == null
                || email.MailAddresses.Count == 0)
                res.Add("code", 0);
            else
            {
                EmailConfiguration emailConfig = await _database.GetEmailConfiguration();
                String md5result = CaculateActuallMd5Value(email, sdt, emailConfig.salt);
                if (!md5result.Equals(md5v,StringComparison.OrdinalIgnoreCase))
                {
                    res.Add("code", 0);
                    res.Add("msg", "md5 is wrong");
                }
                else if (sdt.AddSeconds(emailConfig.liveSeconds).CompareTo(DateTime.UtcNow) < 0)
                {
                    res.Add("code", 0);
                    res.Add("msg", "date time is out of date!");
                }
                else
                {
                    sender.send(email);
                    res.Add("code", 1);
                    res.Add("msg", "ok");
                }
            }
            return Content(JsonConvert.SerializeObject(res));
        }
        private String CaculateActuallMd5Value(Email email, DateTime sdt,String salt) {
            String md5Str = GetMd5Str(email,sdt,salt);
            return CreateMD5(md5Str);
        }
        private string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        private String GetMd5Str(Email email, DateTime sdt, String salt) {
            return String.Format("{0}_{1}_{2}_{3}_{4}_{5}"
                , email.address
                ,email.body
                ,email.subject
                ,email.sendername
                ,sdt.ToString("yyyy-MM-dd HH:mm:ss")
                ,salt);
        }
    }
}
