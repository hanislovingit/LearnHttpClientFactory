using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientFactory.DelegatingHandlers
{
    public class TimeOutDelegatingHandler: DelegatingHandler
    {
        private readonly TimeSpan _timeOutSpan = TimeSpan.FromSeconds(100);

        public TimeOutDelegatingHandler(TimeSpan timeOutSpan): base()
        {
            _timeOutSpan = timeOutSpan;
        }

        public TimeOutDelegatingHandler(HttpMessageHandler innerHandler, TimeSpan timeOutSpan) : base(innerHandler)
        {
            _timeOutSpan = timeOutSpan;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                linkedCancellationTokenSource.CancelAfter(_timeOutSpan);

                try
                {
                    return await base.SendAsync(request, linkedCancellationTokenSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException("The request timed out", ex);
                    }

                    throw;
                }
            }
        }
    }
}
 