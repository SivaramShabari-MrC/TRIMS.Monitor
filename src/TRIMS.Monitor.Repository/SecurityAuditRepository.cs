using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Dapper;
using TRIMS.Monitor.Entity;
using System.Data.SqlClient;

namespace TRIMS.Monitor.Repository
{
    public class SecurityAuditRepository : ISecurityAuditRepository
    {
        private readonly ILogger _logger;
        private readonly AppSettingsConfig _config;
        public SecurityAuditRepository(AppSettingsConfig config, ILogger<SecurityAuditRepository> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async Task LogAudit(SecurityAudit securityAudit)
        {
            var connection = new SqlConnection(_config.ConnectionStrings.SecurityAuditDb);

            var query = "INSERT INTO tbl_SecurityAudit (UserEmail, DateTime, Action, Environment, SystemType, Description, ThreadName) " +
                        "VALUES (@UserEmail, @DateTime, @Action, @Environment, @SystemType, @Description, @ThreadName)";

            var rowsAffected = await connection.ExecuteAsync(query, securityAudit);

            if (rowsAffected > 0)
            {
                Console.WriteLine("Insert successful.");
            }
            else
            {
                Console.WriteLine("Insert failed.");
            }
        }
    }
}
