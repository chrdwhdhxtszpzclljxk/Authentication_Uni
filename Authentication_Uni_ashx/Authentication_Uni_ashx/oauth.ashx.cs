using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Authentication_Uni_ashx
{
    /// <summary>
    /// oauth 的摘要说明
    /// </summary>
    public class oauth : IHttpHandler
    {

        public void ProcessRequest(HttpContext c)
        {
            c.Response.ContentType = "application/json; charset=utf-8";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            c.Response.AddHeader("Access-Control-Allow-Origin", "http://127.0.0.1:8020");
            c.Response.AddHeader("Access-Control-Allow-Credentials", "true");

            dict["access_token"] = "";
            dict["token_type"] = "";
            dict["expires_in"] = "";
            dict["refresh_token"] = "";
            dict["msg"] = "未知错误";

            String cmd = c.Request["grant_type"];
            if (cmd == "" || cmd == null)
            {
                dict["msg"] = "命令不能为空";

            }
            else if (cmd == "password")
            {
                
            }
            else
            {
                dict["msg"] = "未知命令" + cmd;
            }
            c.Response.Write(serializer.Serialize(dict));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}