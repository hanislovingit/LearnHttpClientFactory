using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LearnHttpClientFactory.Models;
using Newtonsoft.Json;

namespace HttpClientFactory.Services
{
    public class StreamService: IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        private int numberOfRequests = 200;

        public StreamService()
        {
            // set up HttpClient instance
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //await TestGetPosterWithStream();
            //await TestGetPosterWithStreamAndCompletionMode();
            //await TestGetPosterWithoutStream();
            await PostPosterWithStream();
        }

        private async Task PostPosterWithStream()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation()
            {
                Name = "A new poster for the Big Lebowski",
                Bytes = generatedBytes
            };

            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));

                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.HttpHeaderAppJson);

                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var createdContent = await response.Content.ReadAsStringAsync();
                    var createdPoster = JsonConvert.DeserializeObject<Poster>(createdContent);
                }
            }
        }

        private async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
            }
        }

        private async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
            }
        }

        private async Task GetPosterWithoutStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var posters = JsonConvert.DeserializeObject<Poster>(content);
        }

        private async Task TestGetPosterWithoutStream()
        {
            // start stop watch
            var stopWatch = Stopwatch.StartNew();

            // run requests
            for (int i = 0; i < numberOfRequests; i++)
            {
                await GetPosterWithoutStream();
            }

            // stop stop watch
            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds without stream: " +
                              $"{stopWatch.ElapsedMilliseconds}, " +
                              $"averaging {stopWatch.ElapsedMilliseconds / numberOfRequests} milliseconds/request");

        }

        private async Task TestGetPosterWithStream()
        {
            // warmup
            await GetPosterWithStream();

            // start stopwatch 
            var stopWatch = Stopwatch.StartNew();

            // run requests
            for (int i = 0; i < numberOfRequests; i++)
            {
                await GetPosterWithStream();
            }

            // stop stopwatch
            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds with stream: " +
                              $"{stopWatch.ElapsedMilliseconds}, " +
                              $"averaging {stopWatch.ElapsedMilliseconds / numberOfRequests} milliseconds/request");
        }


        private async Task TestGetPosterWithStreamAndCompletionMode()
        {
            // warmup
            await GetPosterWithStreamAndCompletionMode();

            // start stopwatch 
            var stopWatch = Stopwatch.StartNew();

            // run requests
            for (int i = 0; i < numberOfRequests; i++)
            {
                await GetPosterWithStreamAndCompletionMode();
            }

            // stop stopwatch
            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds with stream and completionmode: " +
                              $"{stopWatch.ElapsedMilliseconds}, " +
                              $"averaging {stopWatch.ElapsedMilliseconds / numberOfRequests} milliseconds/request");
        }
    }
}