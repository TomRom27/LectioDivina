using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LectioDivina.OnJestSlowoProxy
{
    public class OnJestSlowoProxy
    {
        public List<Post> GetSimplifiedPost(DateTime day)
        {
            string requestUrl = CreateGetDatePostsRequestUrl(DateToUrlParam(day));

            PostsResponse postsResponse = GetRestJsonObject<PostsResponse>(requestUrl);
            if (postsResponse.count > 0)
                return postsResponse.posts;
            else

                return new List<Post>();
        }


        public void SendPost(string user, string password, string title, string category, DateTime date, string content)
        {
            const String phpDateFormat = "Y-m-d H:i:s";
            const String dateFormat = "yyyy-MM-dd HH:mm:ss";
            string url = CreatePostUrl(user, password, phpDateFormat);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "text/json;charset=utf-8";

            CreatePostRequest postRequest = new CreatePostRequest()
            {
                author = user,
                categories = category,
                content = content,
                status = "publish",
                title = title,
                date = date.ToString(dateFormat)
            };

            // if the post date is not today, we need set proper status
            if ((date - DateTime.Today).TotalDays > 1)
                postRequest.status = "future";



            SerializeJsonRequest<CreatePostRequest>(postRequest, request);


            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var resp = DeserializeJsonResponse<GenericResponse>(response);
                if (!String.IsNullOrEmpty(resp.error))
                    throw new Exception(resp.error);
            }
        }


        public void UploadFile(string user, string password, string filename)
        {
            string url = CreateUploadMediaUrl(user, password);

            System.Net.WebClient client = new System.Net.WebClient();

            client.Headers.Add("Content-Type", "binary/octet-stream");

            byte[] response = client.UploadFile(url, "POST", filename);

            using (var memStream = new MemoryStream(response))
            {
                var resp = DeserializeJsonResponse<GenericResponse>(memStream);
                if (!String.IsNullOrEmpty(resp.error))
                    throw new Exception(resp.error);
            }
        }

        #region JSON de & serialization

        private static T DeserializeJsonResponse<T>(Stream stream) where T : class
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            try
            {
                var resp = ser.ReadObject(stream) as T;
                return resp;
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                throw new Exception("Unexpected response from the server:\r\n" + ex.Message);
            }
        }

        private static T DeserializeJsonResponse<T>(HttpWebResponse response) where T : class
        {
            return DeserializeJsonResponse<T>(response.GetResponseStream());
        }

        private static void SerializeJsonRequest<T>(T obj, HttpWebRequest request) where T : class
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            ser.WriteObject(request.GetRequestStream(), obj);
        }

        private static string SerializeJsonRequest<T>(T obj)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ser.WriteObject(memoryStream, obj);
                return encoding.GetString(memoryStream.ToArray());
            }
        }
        #endregion

        private T GetRestJsonObject<T>(string requestUrl) where T : class
        {
            string cookieName;
            string cookieValue;

            return GetRestJsonObjectAndLoginCookie<T>(requestUrl, out cookieName, out cookieValue);
        }

        private T GetRestJsonObjectAndLoginCookie<T>(string requestUrl, out string cookieName, out string cookieValue) where T : class
        {
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));

                T obj = DeserializeJsonResponse<T>(response);
                ExtractLoginCookie(response, out cookieName, out cookieValue);

                return obj;
            }
        }

        private string CreateGetDatePostsRequestUrl(string dayString)
        {
            return String.Format("http://www.onjest.pl/slowo/?json=get_date_posts&date={0}&include=title,date,content,status",
                dayString);
        }

        private string CreateGetNonceForPostsRequestUrl()
        {
            return "http://www.onjest.pl/slowo/?json=core.get_nonce&controller=posts&method=create_post&callback=";
        }

        private string DateToUrlParam(DateTime day)
        {
            return day.ToString("yyyyMMdd");
        }


        private string CreatePostUrl(string user, string password, string dateFormat)
        {
            string url = String.Format("http://www.onjest.pl/slowo/?json=posts/create_user_post&user_login={0}&user_password={1}&date_format={2}",
                                user, password, dateFormat);

            return url;
        }

        private string CreateUploadMediaUrl(string user, string password)
        {
            string url = String.Format("http://www.onjest.pl/slowo/?json=media/upload_media&user_login={0}&user_password={1}",
                    user, password);

            return url;
        }

        private bool ExtractLoginCookie(HttpWebResponse response, out string cookieName, out string cookieValue)
        {
            cookieName = cookieValue = "";
            if (response.Headers.Count > 0)
            {
                string wpCookieHeader = response.Headers["Set-Cookie"];
                if (!String.IsNullOrEmpty(wpCookieHeader))
                    ExtractLoginCookie(wpCookieHeader, ref cookieName, ref cookieValue);

                return !String.IsNullOrEmpty(cookieName);
            }
            else
                return false;
        }

        private void ExtractLoginCookie(string wpCookieHeader, ref  string cookieName, ref  string cookieValue)
        {
            const string loginCookiePrefix = "wordpress_logged_in_";
            int prefixPos = wpCookieHeader.IndexOf(loginCookiePrefix);
            if (prefixPos > 0)
            {
                // cut out the cookie part only
                wpCookieHeader = GetSubstringToFirstChar(wpCookieHeader.Substring(prefixPos), ';');
                // seprate name & value
                string[] parts = wpCookieHeader.Split(new char[] { '=' });
                if (parts.Length >= 2)
                {
                    cookieName = parts[0];
                    cookieValue = parts[1];
                }
            }

        }

        private string GetSubstringToFirstChar(string s, char c)
        {
            var pos = s.IndexOf(c);
            if (pos >= 0)
                return s.Substring(0, pos - 1);
            else
                return s;

        }

    }
}
