using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;


namespace PandaConsole.Sakai
{
    /// <summary>
    /// This class represents the Sakai user.
    /// </summary>
    public sealed class SakaiUser
    {
        #region Fields

        /// <summary>
        /// The user identifier.
        /// </summary>
        string userId;

        /// <summary>
        /// The user password.
        /// </summary>
        string password;

        /// <summary>
        /// The cookie used for authencation.
        /// </summary>
        CookieContainer cookie;

        /// <summary>
        /// The URL to the login form of Panda.
        /// </summary>
        const string LoginFormUrl = "https://panda.ecs.kyoto-u.ac.jp/portal/login";

        /// <summary>
        /// The URL to the portal of Panda.
        /// </summary>
        const string PortalUrl = "https://panda.ecs.kyoto-u.ac.jp/portal";

        /// <summary>
        /// The URL to retrive the json of site collection.
        /// </summary>
        const string SiteCollectionJsonUrl = "https://panda.ecs.kyoto-u.ac.jp/direct/site.json";

        /// <summary>
        /// The regex object used to post the sign-in information.
        /// </summary>
        static readonly Regex LoginFormRegex = new Regex(@"\<input type=""hidden"" name=""lt"" value=""(.*?)"".*?\>");

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PandaConsole.Sakai.SakaiUser"/> class.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="pass">Password.</param>
        public SakaiUser(string id, string pass)
        {
            userId = id;
            password = pass;
            cookie = new CookieContainer();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Log in panda and get cookie.
        /// </summary>
        public void LogIn()
        {
            var request = CreateRequest(LoginFormUrl);
            var response = request.GetResponse();
            var respHtml = ReadStream(response.GetResponseStream());
            Uri redirectedFormUrl = response.ResponseUri;
            response.Close();
            request.Abort();

            string lt = LoginFormRegex.Match(respHtml).Groups[1].Value;
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

            if (postRedirectedUrl != PortalUrl)
            {
                throw new Exception("Wrong username or password!");
            }
        }

        /// <summary>
        /// Gets user's site collection.
        /// </summary>
        /// <returns>The sites of the authrized user.</returns>
        public SakaiSiteCollection GetSites()
        {
            string siteJson = GetHttpText(SiteCollectionJsonUrl);
            return SakaiSiteCollection.Create(siteJson);
        }

        /// <summary>
        /// Gets the resources of the specified site.
        /// </summary>
        /// <returns>The resources of the specified site.</returns>
        /// <param name="site">The site.</param>
        public SakaiResourceCollection GetResources(SakaiSite site)
        {
            string resourceJson = GetHttpText($"https://panda.ecs.kyoto-u.ac.jp/direct/content/site/{site.Id}.json");
            return SakaiResourceCollection.CreateFromJson(resourceJson);
        }

        /// <summary>
        /// Downloads the specified resource and saves to the specified path.
        /// </summary>
        /// <param name="resource">Resource to download.</param>
        /// <param name="savepath">Where to save the file.</param>
        public void DownloadResource(SakaiResource resource, string savepath)
        {
            HttpWebRequest request = CreateRequest(resource.Url);
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                SaveStream(stream, savepath);
                stream?.Close();
            }
        }
		
        #endregion
		
        #region Private Methods

        /// <summary>
        /// Creates the <see cref="HttpWebRequest"/> object to specified url with cookies.
        /// </summary>
        /// <returns>The request object.</returns>
        /// <param name="url">The URL to send the request.</param>
        HttpWebRequest CreateRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookie;
            return req;
        }

        /// <summary>
        /// Retrives the string from the specified stream.
        /// </summary>
        /// <returns>The string rad from the stream.</returns>
        /// <param name="stream">The stream to read.</param>
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

        /// <summary>
        /// Retrives data from the specified stream and saves to the specified path.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <param name="savePath">Save path.</param>
        void SaveStream(Stream stream, string savePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath) ?? throw new InvalidOperationException());
            }
            var buffer = new byte[1024];
            using (var writer = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                int length;
                do
                {
                    length = stream.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, length);
                } while (length != 0);
                writer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Send a http request and returns data as text.
        /// </summary>
        /// <returns>The http text.</returns>
        /// <param name="url">The URL to send a request.</param>
        string GetHttpText(string url)
        {
            HttpWebRequest request = CreateRequest(url);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            return ReadStream(responseStream);
        }

        #endregion
    }
}
