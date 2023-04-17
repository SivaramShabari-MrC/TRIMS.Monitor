using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Repository
{
    public class BAIFileStatusRepository : IBAIFileStatusRepository
    {
        private readonly IConfiguration _config;
        public BAIFileStatusRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<ApplicationSetting>> GetAll(EnvironmentType environment)
        {
            var applicationSettings = await Utils.GetTRIMSDbConnection(environment, _config)
                .QueryAsync<ApplicationSetting>("SELECT * FROM tbl_applicationsetting");
            return applicationSettings;
        }

        public async Task<IEnumerable<ApplicationSetting>> GetApplicationSettingsStartsWith(EnvironmentType environment, string startsWith)
        {
            var applicationSettings = await Utils.GetTRIMSDbConnection(environment, _config)
                .QueryAsync<ApplicationSetting>("SELECT * FROM tbl_applicationsetting WHERE settingcode LIKE @startsWith", new { startsWith = startsWith + "%" });
            return applicationSettings;
        }

        public async Task<ApplicationSetting> Get(EnvironmentType environment, string settingCode)
        {
            var applicationSetting = await Utils.GetTRIMSDbConnection(environment, _config)
                .QueryAsync<ApplicationSetting>("SELECT * FROM tbl_applicationsetting WHERE settingcode = @settingCode", new { settingCode });
            return applicationSetting.First();
        }

        public async Task<IEnumerable<USP_CheckPrioryDayFile>> CheckPriorDayFileExists(EnvironmentType environment, DateTime previousBusinessDate, string originatorIDList)
        {
            var priordayFiles = await Utils.GetTRIMSDbConnection(environment, _config)
                .QueryAsync<USP_CheckPrioryDayFile>("exec [dbo].[usp_CheckPrioryDayFile] @PreviousBusinessDate, @OriginatorIDList", new { previousBusinessDate, originatorIDList });
            return priordayFiles;
        }

        public async Task<bool> CheckPriorDayHoliday(EnvironmentType environment, DateTime inputDate)
        {
            var holidays = await Utils.GetTRIMSDbConnection(environment, _config)
                .QueryAsync<Holiday>("SELECT * FROM tbl_holiday");
            bool result = holidays.Any(h => h.HYear == inputDate.Year && h.HDay == inputDate.Date);
            return result;
        }

        public async Task<List<BAIFileStatusResponse>> ReplaceRoutingNumbersWithBankName(EnvironmentType environment, List<BAIFileStatusResponse> list)
        {
            var res = list;
            List<string> routingNumbers = new();
            foreach (var item in list)
            {
                bool isNumeric = double.TryParse(item.BankName, out _);
                if (isNumeric)
                    routingNumbers.Add(item.BankName);
            }
            var map = await Utils.GetTRIMSDbConnection(environment, _config)
                .QueryAsync<RoutingNumberBankName>(@"SELECT BankName, bankRoutingNumber  FROM tbl_bank  B
                                                            LEFT JOIN tbl_BankRouting R 
                                                            ON B.bankid=R.bankid
                                                            WHERE R.bankroutingnumber IN @routingNumbers",
                                                            new { routingNumbers });
            for (int i = 0; i < res.Count; i++)
            {
                bool isNumeric = double.TryParse(res[i].BankName, out _);
                if (isNumeric)
                {
                    res[i].BankName = map.FirstOrDefault(item => item.BankRoutingNumber == res[i].BankName)?.BankName ?? res[i].BankName;
                }
            }
            return res;
        }


    }
}

