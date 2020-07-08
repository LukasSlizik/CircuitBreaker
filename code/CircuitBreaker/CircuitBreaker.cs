using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace Frontend
{
    public class CircuitBreaker<T> where T : new()
    {
        public CircuitBreaker()
        {
            InitHealthChecks();
        }

        public T Cache { get; set; }
        public List<bool> Checks { get; set; } = new List<bool>();

        private bool IsOnline()
        {
            foreach (var check in Checks)
            {
                Console.Write(check);
            }

            if (Checks.TakeLast(3).Any(c => c == false))
                return false;

            return true;
        }

        public async Task<T> GetData()
        {
            if (IsOnline())
            {
                using var client = new System.Net.Http.HttpClient();
                var request = new System.Net.Http.HttpRequestMessage { RequestUri = new Uri("http://backend/People") };
                var response = await client.SendAsync(request);
                Cache = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }

            return Cache;
        }

        private void InitHealthChecks()
        {
            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += async (source, args) =>
            {
                try
                {
                    using var client = new System.Net.Http.HttpClient();
                    var request = new System.Net.Http.HttpRequestMessage { RequestUri = new Uri("http://backend/health") };
                    var response = await client.SendAsync(request);
                    Checks.Add(response.StatusCode == HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    Checks.Add(false);
                }
            };
            timer.Enabled = true;
        }
    }
}