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
            CancellationToken cancellationToken)
        {
            // There are different options to set up polly

            // option 1
            //return Policy
            //    .Handle<HttpRequestException>()
            //    .Or<TaskCanceledException>()
            //    .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
            //    .RetryAsync(_numberOfRetries)
            //    .ExecuteAsync(() => base.SendAsync(request, cancellationToken));

            // option 2
            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                .RetryAsync(_numberOfRetries);
            return retryPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }
            
    }
}
