using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Repository
{
    public interface IBAIFileStatusRepository
    {
        public Task<IEnumerable<ApplicationSetting>> GetAll(EnvironmentType environment);
        public Task<IEnumerable<ApplicationSetting>> GetApplicationSettingsStartsWith(EnvironmentType environment, string startsWith);
        public Task<ApplicationSetting> Get(EnvironmentType environment, string settingCode);
        public Task<IEnumerable<USP_CheckPrioryDayFile>> CheckPriorDayFileExists(EnvironmentType environment, DateTime previousBusinessDate, string originatorIDList);
        public Task<bool> CheckPriorDayHoliday(EnvironmentType environment, DateTime inputDate);
        public Task<List<BAIFileStatusResponse>> ReplaceRoutingNumbersWithBankName(EnvironmentType environment, List<BAIFileStatusResponse> list);
    }
}
