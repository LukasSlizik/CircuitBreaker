using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public T Cache { get; set; }
        public List<bool> Checks { get; set; } = new List<bool>();

        private bool IsOnline() => Checks.TakeLast(5).All(c => c == true);

        public async Task<T> GetData()
        {
            if (IsOnline())
                Cache = await MakeRequest();

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
                    Checks.Add(success);
                }
                catch (Exception)
                {
                    Console.WriteLine($"{DateTime.Now}: CircuitBreaker - backend is offline");
                    Checks.Add(false);
                }
            };
            timer.Enabled = true;
        }
    }
}