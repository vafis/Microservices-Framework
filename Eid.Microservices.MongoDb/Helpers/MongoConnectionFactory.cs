using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Security.Authentication;
using Eid.Microservices.MongoDb.Models;

namespace Eid.Microservices.MongoDb.Helpers
{
    public class MongoConnectionFactory
    {
        private readonly ILogger<MongoConnectionFactory> _logger;

        public MongoConnectionFactory(ILogger<MongoConnectionFactory> logger)
        {
            _logger = logger;
        }

        public IMongoDatabase GetDatabaseConnection(MongoConnectionSettings connectionSettings)
        {
            if (connectionSettings != null)
            {
                if(string.IsNullOrEmpty(connectionSettings.ConnectionString))
                    throw new ArgumentNullException($"Missing parameters {nameof(MongoConnectionSettings)}.{nameof(MongoConnectionSettings.ConnectionString)}");

                if (string.IsNullOrEmpty(connectionSettings.DatabaseName))
                    throw new ArgumentNullException($"Missing parameters {nameof(MongoConnectionSettings)}.{nameof(MongoConnectionSettings.DatabaseName)}");

                var mongoURL = new MongoUrl(connectionSettings.ConnectionString);
                var settings = MongoClientSettings.FromUrl(mongoURL);

                if (connectionSettings.UseSSL && connectionSettings.SslProtocol != SslProtocols.None)
                {
                    settings.UseSsl = true;
                    settings.SslSettings = new SslSettings() { EnabledSslProtocols = connectionSettings.SslProtocol };
                }

                if (connectionSettings.ConnectionTimeout.HasValue && connectionSettings.ConnectionTimeout.Value > default(int))
                    settings.ServerSelectionTimeout = TimeSpan.FromSeconds((double)connectionSettings.ConnectionTimeout.Value);

                var client = new MongoClient(settings);
                IMongoDatabase db = null;

                try
                {
                    // test connection parameters immediately, throws exception if connection is invalid
                    client.ListDatabases().ToEnumerable();
                    db = client.GetDatabase(connectionSettings.DatabaseName);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, $"Failed connecting to the MongoDB server, connection string [{connectionSettings.ConnectionString}]");
                    throw;
                }

                return db;
            }
            else
                throw new ArgumentNullException($"Missing parameter {nameof(connectionSettings)}");
        }
    }
}
