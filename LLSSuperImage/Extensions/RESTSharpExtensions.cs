﻿using System;
using System.Threading.Tasks;
using RestSharp;

namespace LLSSuperImage.Extensions
{
    public static class RESTSharpExtensions
    {
        private static Task<T> SelectAsync<T>(this RestClient client, IRestRequest request, Func<IRestResponse, T> selector)
        {
            var tcs = new TaskCompletionSource<T>();
            var loginResponse = client.ExecuteAsync(request, r =>
            {
                if (r.ErrorException == null)
                {
                    tcs.SetResult(selector(r));
                }
                else
                {
                    tcs.SetException(r.ErrorException);
                }
            });
            return tcs.Task;
        }

        public static Task<string> GetContentAsync(this RestClient client, IRestRequest request)
        {
            return client.SelectAsync(request, r => r.Content);
        }

        public static Task<IRestResponse> GetResponseAsync(this RestClient client, IRestRequest request)
        {
            return client.SelectAsync(request, r => r);
        }
    }
}
