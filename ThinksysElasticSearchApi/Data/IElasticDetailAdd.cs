using Dapper;
using System.Data.SqlClient;
using System.Data;
using ThinksysElasticSearchApi.Model;

namespace ThinksysElasticSearchApi.Data
{
    public interface IElasticDetailAdd
    {
        int ESDetailAdd(int userId, string elasticId);
    }



    public class ElasticDetailAdd : IElasticDetailAdd
    {
        private readonly IConfiguration _configuration;
        public string DefaultConnection => _configuration.GetConnectionString("DefaultConnection");

        public ElasticDetailAdd(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public int ESDetailAdd(int userId, string elasticId)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(DefaultConnection))
            {
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@UserId", userId);
                queryParameters.Add("@ElasticSearchId", elasticId);
                result = connection.Execute(StoreProcedureDetail.sp_ElasticSearchDetailAdd, queryParameters, commandType: CommandType.StoredProcedure);
                
            }
            return result;
        }
    }
}
