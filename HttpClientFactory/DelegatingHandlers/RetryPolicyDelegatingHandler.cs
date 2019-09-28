using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientFactory.DelegatingHandlers
{
    public class RetryPolicyDelegatingHandler: DelegatingHandler
    {
        private readonly int _maxNumberOfRetries = 3;

        public RetryPolicyDelegatingHandler(int maxNumberOfRetries): base()
        {
            _maxNumberOfRetries = maxNumberOfRetries;
        }

        public RetryPolicyDelegatingHandler(HttpMessageHandler innerHandler, int maxNumberOfRetries): base(innerHandler)
        {
            _maxNumberOfRetries = maxNumberOfRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            for (int i = 0; i < _maxNumberOfRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            return response;
        }
    }
}
