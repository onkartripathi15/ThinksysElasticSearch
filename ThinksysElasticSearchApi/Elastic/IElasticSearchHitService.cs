using Elasticsearch.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ThinksysElasticSearchApi.Model;
using ThinksysElasticSearchApi.Data;

namespace ThinksysElasticSearchApi.Elastic
{


    public interface IElasticSearchHitService
    {
        List<SearchModel> GetDataBySearchText(string keyword);
        string PostData(SearchModel searchModel,int id);
    }





    public class ElasticSearchHitService : IElasticSearchHitService
    {
        private readonly IConnectionToEsService _connectionToEsService;
        private readonly IConfiguration _configuration;
        private readonly ICallAPI _callAPI;
        private readonly IElasticDetailAdd _elasticDetailAdd;

        public string DBName => _configuration.GetSection("ElasticSearch:DBName").Value;
        public string TableName => _configuration.GetSection("ElasticSearch:SearchTable").Value;

        public ElasticSearchHitService(IConnectionToEsService connectionToEsService, IConfiguration configuration, 
            ICallAPI callAPI,IElasticDetailAdd elasticDetailAdd)
        {
            _connectionToEsService = connectionToEsService;
            _configuration = configuration;
            _callAPI = callAPI;
            _elasticDetailAdd = elasticDetailAdd;
        }

        public List<SearchModel> GetDataBySearchText(string keyword)
        {

            var responsedata = _connectionToEsService.EsClient().Search<SearchModel>(s => s
                                                                                      .Index(DBName)
                                                                                      .Size(50)
                                                                                      .Query(q => q
                                                                                           .Match(m => m
                                                                                           .Field(x=>x.Keyword)
                                                                                                 .Query(keyword)
                                                                                                   )
                                                                                           )
                                                                                     );

            //List<SearchModel> models = responsedata.ToList();
            var datasend = (from hits in responsedata.Hits select hits.Source).ToList();

            //return Ok(new { datasend, responsedata.Took });
            return datasend.ToList();
        }

        public string PostData(SearchModel searchModel, int id)
        {

            var responsedata = _connectionToEsService.EsClient().Search<SearchModel>(s => s
                                                                                     .Index(DBName)
                                                                                     .Size(50)
                                                                                     .Query(q => q
                                                                                          .Match(m => m
                                                                                          .Field(y => y.Url)
                                                                                                .Query(searchModel.Url)
                                                                                                  )
                                                                                          )
                                                                                    );

            //List<SearchModel> models = responsedata.ToList();
            var datasend = (from hits in responsedata.Hits select hits.Source).ToList();
            if (datasend != null)
            {
                if (datasend.Count > 0)
                {
                    return "Already Exist!";
                }
            }

            string result=_callAPI.PostMethodAPICall($"{DBName}/{TableName}", searchModel);
            var data = (JObject)JsonConvert.DeserializeObject(result)!;
            string ElasticId= data["_id"]!.Value<string>()!;

            int output=_elasticDetailAdd.ESDetailAdd(id, ElasticId);
            return output > 0 ? "Added sucessfully" : "Something went wrong!";

        }
    }




    public interface ICallAPI
    {
        string PostMethodAPICall<T>(string url, T value);
        string PutMethodAPICall<T>(string url, string id, T value);
        string DeleteMethodAPICall<T>(string url, string id);
    }





    public class CallAPI : ICallAPI
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CallAPI(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public HttpClient DefaultMethod()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration.GetSection("ElasticSearch:BaseUrl").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public string PostMethodAPICall<T>(string url, T value)
        {
            HttpResponseMessage response = DefaultMethod().PostAsJsonAsync(url, value).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        public string PutMethodAPICall<T>(string url, string id, T value)
        {
            HttpResponseMessage response = DefaultMethod().PutAsJsonAsync($"{url}/{id}", value).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        public string DeleteMethodAPICall<T>(string url, string id)
        {
            HttpResponseMessage response = DefaultMethod().DeleteAsync($"{url}/{id}").Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
