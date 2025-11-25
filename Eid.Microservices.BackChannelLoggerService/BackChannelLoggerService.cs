using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Eid.Microservices.BackChannelLoggerService
{
    public class BackChannelLoggerService<TCategoryName> : IBackChannelLoggerService<TCategoryName>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<TCategoryName> _logger;

        public BackChannelLoggerService(IHostingEnvironment hostingEnvironment,
            ILogger<TCategoryName> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning(httpResponseMessage.Content.ReadAsStringAsync().Result);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(null, message);
        }

        public string LogError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return _hostingEnvironment.IsDevelopment() ? ex.Message : string.Empty;
        }

        public string LogError(AggregateException ae)
        {
            ae.Flatten().InnerExceptions.ToList().ForEach(x => _logger.LogError(x, x.Message));
            return _hostingEnvironment.IsDevelopment() ? ae.Message : string.Empty;
        }

        public string LogError(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogError(BuildHttpErrorMessage(httpResponseMessage, out var internalServerError));

            return _hostingEnvironment.IsDevelopment() &&
                   httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError
                ? internalServerError
                : httpResponseMessage.ReasonPhrase;
        }

        private string BuildHttpErrorMessage(HttpResponseMessage httpResponseMessage, out string internalServerError)
        {
            internalServerError = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var error = "*---------- Back Channel Request Error -------------*" + "\r\n";
            error += "\t\t" + "RequestUri: " + httpResponseMessage.RequestMessage.RequestUri.ToString() + "\r\n";
            error += "\t\t" + "StatusCode: " + httpResponseMessage.StatusCode.ToString() + "\r\n";
            error += "\t\t" + "Reason Phraze: " + httpResponseMessage.ReasonPhrase + "\r\n";
            if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                error += "\t\t" + "Internal ServerError Description: " + internalServerError + "\r\n";
            }

            error += "*---------------------------------------------*";

            return error;

        }
    }
}
