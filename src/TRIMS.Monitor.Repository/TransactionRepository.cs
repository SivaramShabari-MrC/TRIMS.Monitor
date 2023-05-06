using Dapper;
using System.Data;
using System.Data.SqlClient;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _connection;
        public TransactionRepository(AppSettingsConfig config)
        {
            _connection = new SqlConnection(config.ConnectionStrings.TRIMS);
        }
        public async Task<List<TransactionReport>> GetTransactionReport(DateTime from, DateTime to)
        {
            
            var result = await _connection.QueryAsync<TransactionReport>(@"SELECT  TransactionTypeID AS type,  
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
