using Microsoft.Extensions.Logging;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Repository;

namespace TRIMS.Monitor.Manager
{
    public class BAIFileStatusManager : IBAIFileStatusManager
    {
        private readonly IBAIFileStatusRepository _repository;
        private const string PRIOR_DAY = "PriorDay";

        public BAIFileStatusManager(IBAIFileStatusRepository repository)
        {
            _repository = repository;
        }
        public async Task<BAIFileStatusResponse[]> CheckFileForDate(DateTime date)
        {
            try
            {
                IEnumerable<ApplicationSetting> BankNames = await _repository.GetApplicationSettingsStartsWith("StatementBAI_");
                ApplicationSetting DisabledPriordayBanks = await _repository.Get("PriordayNotificationDisabledBanks");
                string disabledPriordayBankValue = DisabledPriordayBanks.SettingValue;
                bool status = true;
                List<BAIFileStatusResponse> response = new();
                foreach (ApplicationSetting BankName in BankNames)
                {
                    bool isEnabledPriorDayNotification = await IsEnabledPriorDayNotification(BankName.SettingCode.Split('_')[1], disabledPriordayBankValue);
                    if (isEnabledPriorDayNotification)
                    {
                        var orginatingNumbers = BankName.SettingValue;
                        var statementBAIFileDetail = await _repository.CheckPriorDayFileExists(date, orginatingNumbers);
                        bool isFileExists = statementBAIFileDetail != null && statementBAIFileDetail.Count(e => e.FileType == PRIOR_DAY) > 0;
                        status &= isFileExists;
                        response.Add(new BAIFileStatusResponse { BankName = BankName.SettingValue.Split(",")[0], Status = status ? "File Loaded" : "File Not Loaded" });
                    }
                    else
                    {
                        response.Add(new BAIFileStatusResponse { BankName = BankName.SettingValue.Split(",")[0], Status = "Bank Holiday" });
                    }
                }
                var responseWithBankNames = await _repository.ReplaceRoutingNumbersWithBankName(response);
                return responseWithBankNames.ToArray() ?? Array.Empty<BAIFileStatusResponse>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting statuses for BAI Statement files", ex);
            }
        }
        private async Task<bool> IsEnabledPriorDayNotification(string BankName, string DisabledPriordayBanks)
        {
            if (!DisabledPriordayBanks.Contains(BankName))
                return true;
            var dt = Convert.ToDateTime(Convert.ToDateTime(DateTime.Now.ToString()).ToShortDateString());
            DateTime inputDate = dt.Date;
            if (await _repository.CheckPriorDayHoliday(dt))
            {
                return false;
            }
            return true;
        }
    }
}
