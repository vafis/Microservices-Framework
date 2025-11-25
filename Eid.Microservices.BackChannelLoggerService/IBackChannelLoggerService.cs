using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Eid.Microservices.BackChannelLoggerService
{
    public interface IBackChannelLoggerService<TCategoryName>
    {
        string LogError(Exception ex);
        string LogError(AggregateException ae);
        string LogError(HttpResponseMessage httpResponseMessage);
        void LogWarning(string message);
        void LogWarning(HttpResponseMessage httpResponseMessage);
        void LogInformation(string message);
    }
}
