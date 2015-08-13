using System;
using System.Data;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Hackathon.Business.InstagramProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class Instagram
    {
        private string access_token;

        /// <summary>
        /// Instantiate this method with a valid access token. For helping getting your access token
        /// visit http://instagramdotnet.com
        /// </summary>
        /// <param name="access_token">A valid access token</param>
        public Instagram(string access_token)
        {
            this.access_token = access_token;
        }

        /// <summary>
        /// Returns a JSON string of your resulting query. If the user parameter contains a valid username
        /// the method will return the user information, including the 'id' which you can use against the 
        /// other API endpoints in this class.
        /// </summary>
        /// <param name="user">Instagram username</param>
        /// <returns>string (json)</returns>
        public string GetUserId(string user)
        {
            string output = "";
            try
            {
                WebResponse response = processWebRequest("https://api.instagram.com/v1/media/popular?client_id=YOUR_ID_HERE");

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    JsonTextReader reader = new JsonTextReader(new StringReader(sr.ReadToEnd()));
                    while (reader.Read())
                    {
                        output += reader.Value;
                    }
                }

                return output;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        public DataTable GetMediaRecent(string user_id, int media_count = 50)
        {
            var dt = new DataTable();
            dt.Columns.Add("LargeImage");
            dt.Columns.Add("SmallImage");
            dt.Columns.Add("Likes");
            dt.Columns.Add("Caption");
            dt.Columns.Add("Tags");
            WebResponse response = processWebRequest("https://api.instagram.com/v1/media/popular?client_id=YOUR_ID_HERE");

            using (var sr = new StreamReader(response.GetResponseStream()))
            {

                InstagramObject _instagram = JsonConvert.DeserializeObject<InstagramObject>(sr.ReadToEnd());
                int count = 0;
                int totalPhotos = _instagram.data.Count - 1;
                while (count < totalPhotos)
                {
                    string tags = "";
                    foreach (object o in _instagram.data[count].tags)
                    {
                        tags += "#" + o + ",";
                    }
                    dt.Rows.Add(_instagram.data[count].images.standard_resolution.url, _instagram.data[count].images.low_resolution.url, _instagram.data[count].likes.count, _instagram.data[count].caption.text, tags.TrimEnd(new char[] { ',' }));
                    count = count + 1;
                }
            }
            return dt;
        }

        private WebResponse processWebRequest(string url)
        {
            WebRequest request;
            WebResponse response;

            request = WebRequest.Create(url);
            response = request.GetResponse();

            return response;

        }
    }
}