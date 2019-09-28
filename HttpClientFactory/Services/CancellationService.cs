using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LearnHttpClientFactory.Models;
using Marvin.StreamExtensions;

namespace HttpClientFactory.Services
{
    public class CancellationService: IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient(
            new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public CancellationService()
        {
            // set up HttpClient instance
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 5);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //_cancellationTokenSource.CancelAfter(2000);
            //await GetTrailerAndCancel(_cancellationTokenSource.Token);
            await GetTrailerAndHandleTimeout(_cancellationTokenSource.Token);
        }

        private async Task GetTrailerAndCancel(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(Constants.HttpHeaderEncodingGZip));

            try
            {
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();

                    var trailer = await stream.ReadAndDeserializeFromJsonAsync<Trailer>();
                }
            }
            catch (OperationCanceledException operationCanceledException)
            {
                Console.WriteLine($"an operation was cancelled with msg {operationCanceledException.Message}");
                // additional cleanup
            }
        }

        private async Task GetTrailerAndHandleTimeout(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(Constants.HttpHeaderEncodingGZip));

            try
            {
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();

                    var trailer = await stream.ReadAndDeserializeFromJsonAsync<Trailer>();
                }
            }
            catch (OperationCanceledException operationCanceledException)
            {
                Console.WriteLine($"an operation was cancelled with msg {operationCanceledException.Message}");
                // additional cleanup
            }
        }
    }
}
