using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Extensions
{
    public static class HttpRequestExtension
    {
        public static string BaseUrl(this HttpRequest request) {

                return request.Scheme + "://" + request.Host.Value;
       
        }
    }
}
