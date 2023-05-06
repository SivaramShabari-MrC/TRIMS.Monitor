using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Repository
{
    public interface IBAIFileStatusRepository
    {
        public Task<IEnumerable<ApplicationSetting>> GetAll();
        public Task<IEnumerable<ApplicationSetting>> GetApplicationSettingsStartsWith(string startsWith);
        public Task<ApplicationSetting> Get(string settingCode);
        public Task<IEnumerable<USP_CheckPrioryDayFile>> CheckPriorDayFileExists(DateTime previousBusinessDate, string originatorIDList);
        public Task<bool> CheckPriorDayHoliday(DateTime inputDate);
        public Task<List<BAIFileStatusResponse>> ReplaceRoutingNumbersWithBankName(List<BAIFileStatusResponse> list);
    }
}
