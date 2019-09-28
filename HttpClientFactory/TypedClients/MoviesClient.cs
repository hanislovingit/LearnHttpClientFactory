using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LearnHttpClientFactory.Models;
using Marvin.StreamExtensions;

namespace HttpClientFactory.TypedClients
{
    public class MoviesClient
    {
        private HttpClient _client { get; }

        public MoviesClient(HttpClient client)
        {
            _client = client;

            // the typed clients have transient scope by default.
            // We can set the defaults here for the client instead of in the DI container setup (Program.cs), but we don't 
            // want to override the configured HttpClientHandler bc if we did, those defaults wouldn't apply to handlers in the pool
            _client.BaseAddress = new Uri("http://localhost:57863");
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
        }

        public async Task<IEnumerable<Movie>> GetMovies(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(Constants.HttpHeaderEncodingGZip));

            using (var response = await _client.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
                return movies;
            }
        }
    }
}
