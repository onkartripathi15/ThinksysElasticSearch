using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using ThinksysElasticSearchApi.Elastic;
using ThinksysElasticSearchApi.Model;

namespace ThinksysElasticSearchApi.Controllers
{
    [ApiController]
    public class ElasticSearchHitController : ControllerBase
    {
        private readonly IElasticSearchHitService _elasticSearchHit;
        public  ElasticSearchHitController(IElasticSearchHitService elasticSearchHit)
        {
            _elasticSearchHit = elasticSearchHit;
        }
        [Route("api/[controller]/GetDataBySearchText")]
        [HttpPost]        
        public IActionResult GetDataBySearchText([FromBody] string keyword)
        {            
            keyword = keyword.ToLower().Trim();
            List<SearchModel> searchModel= _elasticSearchHit.GetDataBySearchText(keyword); 
            return Ok(searchModel);
        }

        [Route("api/[controller]/PostData")]
        [HttpPost]
        public IActionResult PostData(SearchModel searchModel)
        {
            
            if (HttpContext.Request.Headers.TryGetValue("UserID", out var extractedApiKey))
            {
                int id = Convert.ToInt32(extractedApiKey);
                if (!string.IsNullOrEmpty(searchModel.Url) && !string.IsNullOrEmpty(searchModel.Keyword) && !string.IsNullOrEmpty(searchModel.Description))
                {
                    var result = _elasticSearchHit.PostData(searchModel, id);
                    return Ok(new
                    {
                        Message = result
                    }) ;
                }
                else
                    return ValidationProblem();
            }
            else
            {
                return Unauthorized();
            }
           
        }
    }
}
