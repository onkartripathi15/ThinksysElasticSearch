using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThinksysElasticSearchApi.Interface;
using ThinksysElasticSearchApi.Model;

namespace ThinksysElasticSearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticSearchHitController : ControllerBase
    {
        private readonly IElasticSearchHit _elasticSearchHit;
        public  ElasticSearchHitController(IElasticSearchHit elasticSearchHit)
        {
            _elasticSearchHit = elasticSearchHit;
        }
        [HttpPost]
        public IActionResult GetDataBySearchText(string keyword)
        {
            keyword = keyword.ToLower().Trim();
           SearchModel searchModel= _elasticSearchHit.GetDataBySearchText(keyword); 
            return Unauthorized();
        }
    }
}
