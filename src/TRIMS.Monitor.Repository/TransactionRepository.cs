using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using TRIMS.Monitor.Entity;
namespace TRIMS.Monitor.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IConfiguration _config;
        public TransactionRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<List<TransactionReport>> GetTransactionReport(EnvironmentType environment, DateTime from, DateTime to)
        {
            var connection = Utils.GetTRIMSDbConnection(environment, _config);
            var result = await connection.QueryAsync<TransactionReport>(@"SELECT  TransactionTypeID AS type,  
	                                                                              banktransactionstatusname AS status, 
	                                                                              COUNT(1) AS 'count'
                                                                          FROM tbl_BankTransaction  T
                                                                          LEFT JOIN tbl_banktransactionstatus S
                                                                          ON T.banktransactionstatusid = S.banktransactionstatusid
                                                                          WHERE valuedate >= @from AND valuedate <= @to 
                                                                          GROUP BY T.TransactionTypeID, BankTransactionStatusname
                                                                          ORDER BY TransactionTypeID",
                                                                          new { from, to });

            return result.ToList();
        }

    }
}
