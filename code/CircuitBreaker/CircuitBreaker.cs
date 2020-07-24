using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace CircuitBreaker
{
    public class CircuitBreaker<T> where T : new()
    {
        public CircuitBreaker()
        {
            InitHealthChecks();
        }

        public ResponseObject<T> Cache { get; set; }
        public List<bool> HealthChecks { get; set; } = new List<bool>();

        private bool IsOnline() => HealthChecks.TakeLast(5).All(c => c == true);

        public async Task<ResponseObject<T>> GetData()
        {
            if (IsOnline())
                Cache = new ResponseObject<T>(await MakeRequest());

            return Cache;
        }

        private async Task<T> MakeRequest()
        {
            using var client = new System.Net.Http.HttpClient();
            var request = new System.Net.Http.HttpRequestMessage { RequestUri = new Uri("http://backend/People") };
            var response = await client.SendAsync(request);

            var responseObject = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(responseObject);
        }

        private void InitHealthChecks()
        {
            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += async (source, args) =>
            {
                try
                {
                    using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                    var request = new System.Net.Http.HttpRequestMessage { RequestUri = new Uri("http://backend/health") };
                    var response = await client.SendAsync(request);
                    var success = response.StatusCode == HttpStatusCode.OK;
                    var msg = success ? "backend is online" : "backend is offline";
                    Console.WriteLine($"{DateTime.Now}: CircuitBreaker - {msg}");
                    HealthChecks.Add(success);
                }
                catch (Exception)
                {
                    Console.WriteLine($"{DateTime.Now}: CircuitBreaker - backend is offline");
                    HealthChecks.Add(false);
                }
            };
            timer.Enabled = true;
        }
    }

    public class ResponseObject<T>
    {
        public ResponseObject(T cache)
        {
            Cache = cache;
            Timestamp = DateTime.Now;
        }
        public DateTime Timestamp { get; set; }
        public T Cache { get; set; }
    }
}