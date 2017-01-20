using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            string strConnetionString = System.Configuration.ConfigurationManager.ConnectionStrings["gtmasqlserver"].ConnectionString;
            int passwordretrytimespan = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["passwordretrytimespan"]);
            int authenticationtimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["authenticationtimeout"]);
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
                string un = c.Request["username"];
                string pwd = c.Request["password"].Substring(8,16);
                string userguid = "";
                //AuthorizationWithAppguid
                dict["msg"] = "用户名或密码错误";
                try
                {
                    int count = 0;
                    object ocount = HttpRuntime.Cache.Get(un);
                    if (ocount != null)
                        count = Convert.ToInt32(ocount);
                    if (count >= 5)
                    {
                        dict["msg"] = "密码错误次数过多，稍后再试。";
                    }
                    else
                    {
                        SqlConnection sc = new SqlConnection(strConnetionString);
                        SqlCommand sm = new SqlCommand();
                        sm.CommandType = CommandType.StoredProcedure;
                        sm.CommandText = "[AuthorizationWithAppguid]";
                        sm.Connection = sc;
                        sm.Parameters.Add(new SqlParameter("@un", un));
                        sm.Parameters.Add(new SqlParameter("@pwd", pwd));
                        sc.Open();
                        SqlDataReader odr = sm.ExecuteReader();
                        if (odr.HasRows)
                        {
                            byte[] solt = Encoding.Default.GetBytes(Guid.NewGuid().ToString().Substring(0, 8));
                            string access_token = "";
                            string refresh_token = "";
                            string jwt_header = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
                            string jwt_payload = "";

                            while (odr.Read())
                            {
                                userguid = odr["userguid"].ToString();
                                jwt_payload = "{";
                                jwt_payload += "\"id\":\"" + userguid + "\",";
                                jwt_payload += "\"un\":\"" + un + "\",";
                                jwt_payload += "\"mg\":\"" + odr["mgroupid"].ToString() + "\"";
                                jwt_payload += "}";
                            }

                            byte[] bytes = Encoding.Default.GetBytes(jwt_header);
                            jwt_header = Convert.ToBase64String(bytes);

                            bytes = Encoding.Default.GetBytes(jwt_payload);
                            jwt_payload = Convert.ToBase64String(bytes);
                            HMACSHA256 hmac = new HMACSHA256(solt);
                            access_token = jwt_header + "." + jwt_payload;
                            bytes = hmac.ComputeHash(Encoding.Default.GetBytes((access_token)));
                            access_token += "." + Convert.ToBase64String(bytes);


                            dict["access_token"] = access_token;
                            dict["token_type"] = "bearer";
                            dict["expires_in"] = 60 * 60 * 2;
                            dict["refresh_token"] = refresh_token;
                            dict["msg"] = "用户登录成功";
                            HttpRuntime.Cache.Insert(userguid, solt, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(authenticationtimeout));
                        }
                        else
                        {
                            count = Convert.ToInt32(HttpRuntime.Cache.Get(un));
                            HttpRuntime.Cache.Insert(un, count + 1,null,System.Web.Caching.Cache.NoAbsoluteExpiration,TimeSpan.FromMinutes(passwordretrytimespan));
                        }
                        odr.Close();
                        sc.Close();
                    }
                }
                catch (Exception e)
                {
                    dict["msg"] = "错误：" + e.Message;
                }

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

    public class jwtpkg{

    }
}