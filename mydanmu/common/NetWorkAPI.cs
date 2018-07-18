//=============================================================
// Copyright (C) 2018-2018
// CLR版本:          4.0.30319.42000
// 机器名称:          DESKTOP-CSLKP88
// 命名空间名称/文件名:    mydanmu.common/NetWorkAPI 
// 创建人:               knva   
// 创建时间:            2018/7/17 16:19:34
//==============================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace mydanmu.common
{
    class NetWorkAPI
    {
        public async Task<String> CreateGetHttpResponse(string url, int timeout, Hashtable dic)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic != null && dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (DictionaryEntry item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            var client = new HttpClient();
            var task = client.GetAsync(new Uri(builder.ToString()));
            task.Wait();
            var response = task.Result;
            var server = response.Headers.Server;

            result = await response.Content.ReadAsStringAsync();
            client.Dispose();
            return result;
        }
        public async Task<String> CreatePostHttpResponse(string url, Hashtable parameters)
        {
            String result = "";
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

            foreach (string key in parameters.Keys)
            {
                paramList.Add(new KeyValuePair<string, string>(key, parameters[key].ToString()));
            }

            var client = new HttpClient();
            var task = client.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList));
            task.Wait();
            var response = task.Result;
            result = await response.Content.ReadAsStringAsync();
            client.Dispose();
            return result;

        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
    }
}
