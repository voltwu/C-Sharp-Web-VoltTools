using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VoltTools.Models;
using VoltTools.Models.Views;

namespace VoltTools.Controllers
{
    public class URLShortenerController : BaseController
    {
        public IActionResult Index([FromServices] BaseView _view)
        {
            return View(_view);
        }
        public async Task<IActionResult> Proxy([FromServices] IDatabase _database)
        {
            String path = await GetPara("path");
            Shortlink target = await _database.GetLinkAsync(lk => lk.short_url.Equals(path));
            if (target == null ||
                String.IsNullOrEmpty(target.target_url) ||
                DateTime.Compare(target.expire, DateTime.UtcNow) < 0)
                return Redirect("/Home/Error");

            return Redirect(target.target_url);
        }

        public async Task<IActionResult> Addshortlink([FromServices] IDatabase _database,[FromServices] ShortUrlAccounter shorter)
        {
            JObject res = new JObject();
            String path = await GetPara("path");
            if (String.IsNullOrEmpty(path)
                || !path.ToLower().StartsWith("https"))
                res.Add("code", 0);
            else
            {
                ShortLinkConfiguration configuration = await _database.GetShortLinkConfiguration();
                List<Int32> IndexTracker = configuration.DeserializeIndexTracker();
                DateTime lastStateTime = configuration.lastStageTime;
                var short_url = shorter.GetAShortLink(configuration.maxAgeMinutes,
                    ref lastStateTime,
                    ref IndexTracker, configuration.defaultUrlLenth);
                configuration.SetIndexTracker(IndexTracker);
                configuration.lastStageTime = lastStateTime;
                var updateResult = await _database.UpdateShortLinkConfiguration(configuration);
                if (updateResult.MatchedCount <= 0)
                {
                    res.Add("code", 0);
                    return Content(JsonConvert.SerializeObject(res));
                }
                Shortlink obj = new Shortlink();
                obj.target_url = path;
                obj.expire = DateTime.Now.AddMinutes(configuration.maxAgeMinutes);
                obj.short_url = short_url;
                await _database.InsertOrReplaceShortLinkAsync(obj);
                res.Add("short_url", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/sl/{short_url}");
                res.Add("expire", obj.expire);
                res.Add("code", 1);
                return Content(JsonConvert.SerializeObject(res));
            }
            return Content(JsonConvert.SerializeObject(res));
        }
    }
    /// <summary>
    /// 说明：
    ///     默认情况下：
    ///     链接有效期为5分钟
    ///     列表为52个字符(abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ)
    ///     长度为3个字符
    ///     计算下的每秒的最大并发量为：468
    /// </summary>
    public class ShortUrlAccounter
    {
        public int maxPos {
            get {
                return arrs[0].Length;
            }
        }
        public List<String[]> arrs = null;
        /// <summary>
        /// 初始化一个ShortUrlAccount实例对象
        /// </summary>
        /// <param name="maxAge"></param>
        public ShortUrlAccounter()
        {
            arrs = new List<string[]>() {
                new string[]{ "X", "s", "H", "Y", "a", "J", "D", "t", "T", "R", "l", "O", "E", "M", "C", "v", "k", "F", "w", "U", "m", "c", "P", "j", "g", "f", "L", "d", "Q", "q", "G", "Z", "u", "o", "N", "n", "z", "x", "V", "B", "K", "S", "b", "e", "r", "I", "A", "W", "y", "h", "i", "p" }
                ,new string[]{ "u", "g", "i", "q", "R", "Y", "G", "n", "t", "z", "d", "m", "A", "C", "p", "X", "F", "x", "L", "J", "K", "Q", "H", "o", "S", "s", "a", "O", "k", "l", "D", "j", "T", "e", "c", "r", "W", "w", "V", "M", "E", "B", "N", "h", "v", "I", "U", "b", "f", "y", "Z", "P" }
                ,new string[]{ "N", "s", "E", "H", "S", "G", "A", "F", "Y", "n", "c", "K", "o", "t", "C", "D", "O", "v", "B", "w", "y", "l", "T", "J", "Q", "X", "d", "u", "M", "m", "e", "L", "R", "Z", "P", "f", "k", "q", "g", "V", "x", "z", "b", "p", "U", "a", "I", "j", "r", "i", "W", "h" }
                ,new string[]{ "M", "Z", "A", "e", "m", "J", "h", "v", "o", "b", "K", "n", "U", "i", "F", "S", "W", "I", "u", "B", "g", "x", "c", "O", "d", "H", "L", "z", "l", "y", "X", "Q", "R", "a", "t", "q", "D", "p", "P", "T", "r", "w", "V", "E", "f", "N", "k", "Y", "C", "j", "G", "s" }
            };
        }
        public String GetAShortLink(int maxAgeMinutes,ref DateTime lastStageTime,ref List<int> IndexTracker, int defaultUrlLenth) {

            if (DateTime.Compare(lastStageTime.AddMinutes(maxAgeMinutes), DateTime.UtcNow) < 0)
            {
                lastStageTime = DateTime.UtcNow;
                IndexTracker.Clear();
                IndexTracker.AddRange(new Int32[defaultUrlLenth]);
            }

            IncrePos(maxAgeMinutes, lastStageTime, IndexTracker);
            String res = getCorrespondingRepresent(IndexTracker);
            return res;
        }
        private void IncrePos(int maxAgeMinutes, DateTime lastStageTime, List<int> IndexTracker)
        {
            for (int pos = 0; pos < IndexTracker.Count(); pos++) {
                int currentValue = IndexTracker[pos];
                currentValue++;
                if (currentValue >= maxPos)
                {
                    if (pos + 1 >= IndexTracker.Count())
                    {
                        IndexTracker.Add(0);
                        break;
                    }
                    else
                        IndexTracker[pos] = 0;
                }
                else
                {
                    IndexTracker[pos] = currentValue;
                    break;
                }
            }
        }
        private String getCorrespondingRepresent(List<Int32> IndexTracker) {
            String result = String.Empty;
            for (var index = 0; index < IndexTracker.Count(); index++) {
                var pos = IndexTracker[index];
                var ArrPos = index < (arrs.Count() - 1) ? index : (arrs.Count() - 1);
                result += arrs[ArrPos][pos];
            }
            return result;
        }
    }
}
