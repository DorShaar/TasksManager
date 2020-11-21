using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tasker.Communication
{
    public static class HttpMessageRequester
    {
        public static async Task<T> SendHttpRequestMessage<T>(HttpClient httpClient, HttpRequestMessage httpRequestMessage, ILogger logger)
        {
            using HttpResponseMessage response =
                await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

            logger.LogTrace($"Operation {httpRequestMessage.Method.Method} ends with response status: {response.StatusCode}");

            string responseStringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    logger.LogError($"Resource not found, Status code: {response.StatusCode}");
                    return default;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    logger.LogError($"Could not perform operation, result message: {responseStringContent}");
                    return default;
                }

                throw new InvalidOperationException($"Could not perform {httpRequestMessage.Method.Method} operation. "
                    + $"Response status: {response.StatusCode} "
                    + $"Response message: {responseStringContent}");
            }

            return JsonConvert.DeserializeObject<T>(responseStringContent);
        }
    }
}