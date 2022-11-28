using Dapper;
using Microsoft.Extensions.Configuration;
using Nest;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ThinksysElasticSearchApi.Model;

namespace ThinksysElasticSearchApi.Data
{
    public interface IMatchApiKey
    {
        UserModel MatchApiKeyByDB(string XApiKey);
    }




    public class MatchApiKey : IMatchApiKey
    {
        private readonly IConfiguration _configuration;
        public string DefaultConnection => _configuration.GetConnectionString("DefaultConnection");

        public MatchApiKey(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public UserModel MatchApiKeyByDB(string _XApiKey)
        {
            UserModel userModel = new UserModel();
            using (SqlConnection connection = new SqlConnection(DefaultConnection))
            {
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@XApiKey", _XApiKey);
                var results =connection.ExecuteReader(StoreProcedureDetail.sp_UserDetailGetByXApiKey, queryParameters, commandType: CommandType.StoredProcedure);
                DataTable dt = new DataTable();
                dt.Load(results);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        userModel.Id = Convert.ToInt32(dt.Rows[0]["ID"]);
                        userModel.XApiKey = dt.Rows[0]["XApiKey"].ToString()!;

                    }
                }
            }
            return userModel;
        }
    }
}
