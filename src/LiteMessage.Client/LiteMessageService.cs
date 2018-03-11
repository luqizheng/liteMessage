using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace LiteMessage.Client
{

    /// <summary>
    /// 
    /// </summary>
    public class LiteMessageService
    {
        private readonly string appKey;
        private readonly string host;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="host"></param>
        public LiteMessageService(string appKey, string host)
        {
            if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentException("appKey", nameof(appKey));
            }

            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException("host", nameof(host));
            }

            this.appKey = appKey;
            this.host = host.TrimEnd('/') + "/api/message/";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Add(Message message)
        {
            var client = new HttpClient();
            var httpContent = GetContent(client, message);
            var response = client.PostAsync(host, httpContent).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                message.Id = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                return;
            }

            throw new Exception("Add error " + response.Content.ReadAsStringAsync().Result);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <param name="user"></param>
        public void Mark(bool read, string user) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Update(Message message)
        {
            var client = new HttpClient();
            var httpContent = GetContent(client, message);
            var response = client.PutAsync(host + message.Id, httpContent).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }

            throw new Exception("Update error " + response.Content.ReadAsStringAsync().Result);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>

        public void Delete(int id)
        {

            var client = new HttpClient();
            var url = host + id;
            client.DefaultRequestHeaders.Add("signature", Md5(url + appKey));
            var response = client.DeleteAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }

            throw new Exception("Delete error " + response.Content.ReadAsStringAsync().Result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Message Get(int id)
        {

            var client = new HttpClient();
            var url = host + id;
            client.DefaultRequestHeaders.Add("signature", Md5(url + appKey));
            var response = client.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<Message>(response.Content.ReadAsStringAsync().Result);
            }

            throw new Exception("get error " + response.Content.ReadAsStringAsync().Result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public int Count(string search = null)
        {

            var client = new HttpClient();
            var url = host + "count?search=" + search;
            client.DefaultRequestHeaders.Add("signature", Md5(url + appKey));
            var response = client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<int>(json);
            }

            throw new Exception("get error " + json);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public IEnumerable<Message> List(int page, int size, string searchKey = null)
        {
            var client = new HttpClient();
            var url = host + "list?page=" + page + "&pageSize=" + size + "&searchKey=" + searchKey;
            client.DefaultRequestHeaders.Add("signature", Md5(url + appKey));
            var response = client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Message>>(json);
                }
                catch (Exception ex)
                {
                    throw new Exception("format error " + json, ex);
                }
            }

            throw new Exception("list error " + json);
        }

        /// <summary>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private HttpContent GetContent(HttpClient client, object transaction)
        {
            var postData = JsonConvert.SerializeObject(transaction);

            client.DefaultRequestHeaders.Add("signature", Md5(postData + this.appKey));
            var httpContent = new StringContent(postData, Encoding.UTF8,
                "application/json");

            return httpContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        private string Md5(string inputValue)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}
