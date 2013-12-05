using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using LLSSuperImage.Extensions;
using LLSSuperImage.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace LLSSuperImage.Framework
{
    public class DataService
    {
        #region Constructor
        private static DataService s_Instance;
        private static object s_InstanceSync = new object();

        protected DataService()
        {
        }

        /// <summary>
        /// Returns an instance (a singleton)
        /// </summary>
        /// <returns>a singleton</returns>
        /// <remarks>
        /// This is an implementation of the singelton design pattern.
        /// </remarks>
        public static DataService GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (s_Instance == null)
            {
                lock (s_InstanceSync)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new DataService();
                    }
                }
            }
            return s_Instance;
        }
        #endregion

        public async Task<List<Event>> GetEventsAsync(double latitude, double longitude, int distance)
        {
            RestClient restClient = new RestClient("http://ws.audioscrobbler.com/2.0/");
            RestRequest request = new RestRequest();

            request.AddParameter("api_key", "407778b07dd8396a3c3e072848438890");
            request.AddParameter("method", "geo.getEvents");
            request.AddParameter("format", "JSON");
            request.AddParameter("lat", latitude.ToString(new CultureInfo("en-US")));
            request.AddParameter("long", longitude.ToString(new CultureInfo("en-US")));
            request.AddParameter("distance", distance.ToString());
            request.AddParameter("limit", "50");

            List<Event> eventList = new List<Event>();

            string jsonResponse = await restClient.GetContentAsync(request);

            ErrorResponse error = await JsonConvert.DeserializeObjectAsync<ErrorResponse>(jsonResponse);
            if (error.error == 0)
            {
                jsonResponse = jsonResponse.Replace("{\"event\"", "{\"eventlist\"");
                jsonResponse = jsonResponse.Replace("@attr", "attr");
                jsonResponse = jsonResponse.Replace("#text", "text");
                jsonResponse = jsonResponse.Replace("geo:point", "geopoint");
                jsonResponse = jsonResponse.Replace("geo:lat", "geolat");
                jsonResponse = jsonResponse.Replace("geo:long", "geolong");

                JObject eventsParsed = JObject.Parse(jsonResponse);
                var eventJson = eventsParsed["events"]["eventlist"];

                if (!ReferenceEquals(eventJson, null))
                {
                    if (eventJson is JArray)
                    {
                        GetEventsResponse response = await JsonConvert.DeserializeObjectAsync<GetEventsResponse>(jsonResponse);
                        eventList.AddRange(response.events.eventlist);
                    }
                    else
                    {
                        Event eventItem = eventJson.ToObject<Event>();
                        eventList.Add(eventItem);
                    }
                }
            }

            return eventList;
        }
    }
}
