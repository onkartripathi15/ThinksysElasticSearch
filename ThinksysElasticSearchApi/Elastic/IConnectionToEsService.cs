using Elasticsearch.Net;
using Nest;

namespace ThinksysElasticSearchApi.Elastic
{
    public interface IConnectionToEsService
    {
        ElasticClient EsClient();
    }

    public class ConnectionToEsService : IConnectionToEsService
    {
        private readonly IConfiguration _configuration;

        public ConnectionToEsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #region Connection string to connect with Elasticsearch

        public ElasticClient EsClient()
        {
            var nodes = new Uri[]
            {
                new Uri(_configuration.GetSection("ElasticSearch:BaseUrl").Value),
            };

            var connectionPool = new StaticConnectionPool(nodes);
            var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming();
            var elasticClient = new ElasticClient(connectionSettings);

            return elasticClient;
        }

        #endregion Connection string to connect with Elasticsearch
    }
}
