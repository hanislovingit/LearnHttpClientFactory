using HttpClientFactory.TypedClients;
using LearnHttpClientFactory.Models;
using Marvin.StreamExtensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientFactory.Services
{
    public class HttpClientFactoryInstanceMgmtService: IIntegrationService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MoviesClient _moviesClient;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public async Task Run()
        {
            //await GetMoviesWithHttpClientFromFactory(_cancellationTokenSource.Token);
            //await GetMoviesWithNamedHttpClientFromFactory(_cancellationTokenSource.Token);
            //await GetMoviesWithTypedHttpClientFromFactory(_cancellationTokenSource.Token);
            await GetMoviesViaMoviesClient(_cancellationTokenSource.Token);
        }

        public HttpClientFactoryInstanceMgmtService(IHttpClientFactory httpClientFactory, MoviesClient moviesClient)
        {
            _httpClientFactory = httpClientFactory;
            _moviesClient = moviesClient;
        }


        private async Task GetMoviesWithHttpClientFromFactory(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "http://localhost:57863/api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
            }
        }

        private async Task GetMoviesWithNamedHttpClientFromFactory(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(Constants.HttpHeaderEncodingGZip));

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
            }
        }

        //private async Task GetMoviesWithTypedHttpClientFromFactory(CancellationToken cancellationToken)
        //{
        //    var request = new HttpRequestMessage(
        //        HttpMethod.Get,
        //        "api/movies");
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.HttpHeaderAppJson));
        //    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(Constants.HttpHeaderEncodingGZip));

        //    using (var response = await _moviesClient.Client.SendAsync(request,
        //        HttpCompletionOption.ResponseHeadersRead,
        //        cancellationToken))
        //    {
        //        var stream = await response.Content.ReadAsStreamAsync();
        //        response.EnsureSuccessStatusCode();
        //        var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
        //    }
        //}

        private async Task GetMoviesViaMoviesClient(CancellationToken cancellationToken)
        {
            var movies = await _moviesClient.GetMovies(cancellationToken);
        }
    }
}
