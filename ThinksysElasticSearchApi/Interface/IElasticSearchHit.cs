using ThinksysElasticSearchApi.Model;

namespace ThinksysElasticSearchApi.Interface
{
    public interface IElasticSearchHit
    {
        SearchModel GetDataBySearchText(string keyword);
    }
}
