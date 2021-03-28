using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VoltTools.Controllers
{
    public class BaseController : Controller
    {
        private Dictionary<String, String> m_para = null;

        private async Task<Dictionary<String, String>> Para()
        {
                if (m_para != null)
                {
                    return m_para;
                }
                m_para = new Dictionary<string, string>();
                //parsing url
                String uri = Request.QueryString.ToString();
                if (uri == null)
                {
                    return null;
                }
                int quotIndex = uri.IndexOf("?");
                //GET
                if (quotIndex >= 0)
                {
                    String m_uri = String.Empty;
                    try
                    {
                        m_uri = uri.Substring(quotIndex + 1);
                    }
                    catch (ArgumentOutOfRangeException)
                    {//if extract failed,then restore
                        quotIndex = -1;
                        m_uri = uri;
                    }
                    if (quotIndex != -1)
                    {
                        String[] uri_param = m_uri.Split(new Char[] { '&' });
                        foreach (String uri_p in uri_param)
                        {
                            if (uri_p.IndexOf("=") >= 0)
                            {
                                String[] e_param = uri_p.Split(new Char[] { '=' });
                                if (String.IsNullOrEmpty(e_param[0].Trim()))
                                {
                                    continue;
                                }
                                else
                                {
                                    //escape processing
                                    m_para.Add(HttpUtility.UrlDecode(e_param[0]), HttpUtility.UrlDecode(e_param[1]));
                                }
                            }
                        }
                    }
                }
                //POST
                System.IO.Stream streamReceive = this.Request.Body;
                if (streamReceive != null)
                {
                    Encoding encoding = Encoding.UTF8;
                    System.IO.StreamReader streamReader = new System.IO.StreamReader(streamReceive, encoding);
                    string textRead = await streamReader.ReadToEndAsync();
                    if (
                        this.Request.ContentType!=null 
                        && this.Request.ContentType.Contains("application/json")) {
                        m_para = JsonConvert.DeserializeObject<Dictionary<String, String>>(textRead);
                    }
                }
                return m_para;
        }
        internal async Task<String> GetPara(String key, String defaultValue = "")
        {
            (await Para()).TryGetValue(key, out defaultValue);
            return defaultValue;
        }
    }
}
