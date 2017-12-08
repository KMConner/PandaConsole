using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;


namespace PandaConsole
{
    public sealed class SakaiUser
    {
        string userId;

        string password;

        CookieContainer cookie;

        const string LOGIN_FORM_URL = "https://panda.ecs.kyoto-u.ac.jp/portal/login";

        const string PORTAL_URL = "https://panda.ecs.kyoto-u.ac.jp/portal";

        const string SITE_COLLECTION_XML_URL = "https://panda.ecs.kyoto-u.ac.jp/direct/site.xml";

        static readonly Regex loginFormRegex = new Regex(@"\<input type=""hidden"" name=""lt"" value=""(.*?)"".*?\>");

        public SakaiUser(string id, string pass)
        {
            userId = id;
            password = pass;
            cookie = new CookieContainer();
        }

        public void LogIn()
        {
            var request = CreateRequest(LOGIN_FORM_URL);
            var response = request.GetResponse();
            var respHtml = ReadStream(response.GetResponseStream());
            response.Close();
            request.Abort();
            Uri redirectedFormUrl = response.ResponseUri;

            var mm = loginFormRegex.Match(respHtml);

            string lt = loginFormRegex.Match(respHtml).Groups[1].Value;
            var postRequest = CreateRequest(redirectedFormUrl.ToString());
            postRequest.Method = "POST";
            postRequest.ContentType = "application/x-www-form-urlencoded";
            var postData = $"_eventId=submit&execution=e1s1&lt={lt}&password={password}&submit=%E3%83%AD%E3%82%B0%E3%82%A4%E3%83%B3&username={userId}";
            using (var requestStream = postRequest.GetRequestStream())
            {
                var data = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            var postResp = postRequest.GetResponse();

            string postRedirectedUrl = postResp.ResponseUri.ToString();

            postRequest.Abort();

            if (postRedirectedUrl != PORTAL_URL)
            {
                throw new Exception("Wrong username or password!");
            }
        }

        public SakaiSiteCollection GetSites()
        {
            string siteJson = GetHttpText("https://panda.ecs.kyoto-u.ac.jp/direct/site.json");
            return SakaiSiteCollection.Create(siteJson);
        }

        HttpWebRequest CreateRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookie;
            return req;
        }

        string ReadStream(Stream stream)
        {
            string result;
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            return result;
        }

        string GetHttpText(string url)
        {
            HttpWebRequest request = CreateRequest(url);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            return ReadStream(responseStream);
        }
    }
}
