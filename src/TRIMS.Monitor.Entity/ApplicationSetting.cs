namespace TRIMS.Monitor.Entity
{
    public class ApplicationSetting
    {
        public int SettingID { get; set; }

        public string SettingCode { get; set; } = string.Empty;

        public string SettingDesc { get; set; } = string.Empty;

        public string SettingValue { get; set; } = string.Empty;

        public DateTime UpdateDate { get; set; }

        public string UpdatedBy { get; set; } = string.Empty;

    }

    public class USP_CheckPrioryDayFile
    {
        public string FileType { get; set; }
        public string SenderID { get; set; }

    }

    public class Holiday
    {
        public int HYear { get; set; }

        public DateTime HDay { get; set; }

        public byte BankRoutingTypeID { get; set; }

    }

    public class RoutingNumberBankName
    {
        public string BankName { get; set; } = string.Empty;
        public string BankRoutingNumber { get; set; } = string.Empty;
    }

}
