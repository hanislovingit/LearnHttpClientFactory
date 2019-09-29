using Polly;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientFactory.DelegatingHandlers
{
    public class PollyRetryDelegatingHandler : DelegatingHandler
    {
        private readonly int _numberOfRetries;

        public PollyRetryDelegatingHandler(int numberOfRetries) : base()
        {
            _numberOfRetries = numberOfRetries;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                .RetryAsync(_numberOfRetries)
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}
