using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAM.Helpers.Common
{
    /// <summary>
    /// REST API Wrapper for FCM Rich Push Notification by using the Firebase cloud API.
    /// </summary>
    public class FirebaseAPI
    {
        readonly HttpClient client;
        readonly string authKey;

        string url = "https://fcm.googleapis.com/fcm/send";

        /// <summary>
        /// Initialize the FirebaseAPI object, by providing an HttpClient instance and the
        /// Auth key used to communicate with the server to send to correct Tokens/Topics.
        /// </summary>
        /// <param name="client">Httpclient instance which will be used to make the POST operations.</param>
        /// <param name="authKey">Authentication key, which will be passed as key={Your key here}</param>
        public FirebaseAPI(HttpClient client, string authKey)
        {
            this.client = client;
            this.authKey = authKey;
        }

        /// <summary>
        /// Sends rich push notifications using topics.
        /// </summary>
        /// <param name="topic">The topic to be sent to, example: /topics/{your topic here}</param>
        /// <param name="title">Title of the notification.</param>
        /// <param name="body">Text body of the notification.</param>
        /// <returns>Message Id. or errors, if found.</returns>
        public async Task<string> SendPushNotificationTopics(string topic, string title, string body)
        {
            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                string bodyJson =
                    "{ \"to\":\"/topics/" + topic +
                    "\",\"notification\":{" +
                    "\"title\":\"" + title + "\"," +
                    "\"body\":\"" + body + "\"," +
                    "\"mutable_content\": true," +
                    "\"sound\":\"Tri-tone\" } }";
                message.Headers.TryAddWithoutValidation($"Authorization", $"key={authKey}");
                message.Content = new StringContent(bodyJson);
                message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.SendAsync(message);
                var fullRes = await res.Content.ReadAsStringAsync();
                return fullRes;
            }
        }

        /// <summary>
        /// Sends rich push notifications using topics.
        /// </summary>
        /// <param name="topic">The topic to be sent to, example: /topics/{your topic here}</param>
        /// <param name="title">Title of the notification.</param>
        /// <param name="body">Text body of the notification.</param>
        /// <param name="imgUrl">The image to be sent with the notification.</param>
        /// <returns>Message Id. or errors, if found.</returns>
        public async Task<string> SendPushNotificationTopics(string topic, string title, string body, string imgUrl)
        {
            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                string bodyJson =
                    "{ \"to\":\"/topics/" + topic +
                    "\",\"notification\":{" +
                    "\"title\":\"" + title + "\"," +
                    "\"body\":\"" + body + "\"," +
                    "\"mutable_content\": 1," +
                    "\"sound\":\"Tri-tone\", " +
                    "\"image\":\"" + imgUrl + "\"} }";
                message.Headers.TryAddWithoutValidation($"Authorization", $"key={authKey}");
                message.Content = new StringContent(bodyJson);
                message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.SendAsync(message);
                var fullRes = await res.Content.ReadAsStringAsync();
                return fullRes;
            }
        }

        /// <summary>
        /// Sends rich push notifications using tokens.
        /// </summary>
        /// <param name="token">The token to be sent to.</param>
        /// <param name="title">Title of the notification.</param>
        /// <param name="body">Text body of the notification.</param>
        /// <returns>Message Id. or errors, if found.</returns>
        public async Task<string> SendPushNotificationTokens(string token, string title, string body)
        {
            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                string bodyJson =
                    "{ \"to\":\"" + token +
                    "\",\"notification\":{" +
                    "\"title\":\"" + title + "\"," +
                    "\"body\":\"" + body + "\"," +
                    "\"mutable_content\": true," +
                    "\"sound\":\"Tri-tone\" } }";
                message.Headers.TryAddWithoutValidation($"Authorization", $"key={authKey}");
                message.Content = new StringContent(bodyJson);
                message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.SendAsync(message);
                var fullRes = await res.Content.ReadAsStringAsync();
                return fullRes;
            }
        }

        /// <summary>
        /// Sends rich push notifications using tokens.
        /// </summary>
        /// <param name="token">The token to be sent to.</param>
        /// <param name="title">Title of the notification.</param>
        /// <param name="body">Text body of the notification.</param>
        /// <param name="imgUrl">The image to be sent with the notification.</param>
        /// <returns>Message Id. or errors, if found.</returns>
        public async Task<string> SendPushNotificationTokens(string token, string title, string body, string imgUrl)
        {
            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                string bodyJson =
                    "{ \"to\":\"" + token +
                    "\",\"notification\":{" +
                    "\"title\":\"" + title + "\"," +
                    "\"body\":\"" + body + "\"," +
                    "\"mutable_content\": 1," +
                    "\"sound\":\"Tri-tone\", " +
                    "\"image\":\"" + imgUrl + "\"} }";
                message.Headers.TryAddWithoutValidation($"Authorization", $"key={authKey}");
                message.Content = new StringContent(bodyJson);
                message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.SendAsync(message);
                var fullRes = await res.Content.ReadAsStringAsync();
                return fullRes;
            }
        }
    }
}
