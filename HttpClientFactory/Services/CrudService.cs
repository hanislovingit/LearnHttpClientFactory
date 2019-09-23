using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LearnHttpClientFactory.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttpClientFactory.Services
{
    public class CRUDService: IIntegrationService
    {
        private readonly ILogger<CRUDService> _logger;
        private static HttpClient _httpClient = new HttpClient();

        public CRUDService(ILogger<CRUDService> logger)
        {
            _logger = logger;

            // set up httpClient
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
        }

        public async Task Run()
        {
            _logger.LogInformation("hello from CRUDService");

            //await GetResource();
            await GetResourceThroughHttpRequestMsg();
        }

        public async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/movies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = new List<Movie>();

            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            }
            else if (response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Movie>));
                movies = (List<Movie>) xmlSerializer.Deserialize(new StringReader(content));
            }
        }

        public async Task GetResourceThroughHttpRequestMsg()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = new List<Movie>();
            movies = JsonConvert.DeserializeObject<List<Movie>>(content);
        }
    }
}