using Nest;
using Newtonsoft.Json;

namespace ThinksysElasticSearchApi.Model
{
    public class SearchModel
    {
        [JsonProperty(PropertyName = "order")]
        public long Order { get; set; }
        [JsonProperty(PropertyName = "keyword")]
        public string? Keyword { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string? Url { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }

    }
}
