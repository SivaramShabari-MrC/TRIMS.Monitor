using Newtonsoft.Json;
using System.Diagnostics;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Service
{
    public class ScheduledTaskService : IScheduledTaskService
    {
        public ScheduledTaskService() { }
        public async Task<ScheduledTask[]> GetScheduledTasks(string serverName)
        {
            string powerShellCommand = $@"/C invoke-command -ComputerName {serverName} -ScriptBlock {{ " +
                                @"Get-ScheduledTask  -TaskPath ""\STAR.Corporate.TRIMS*"" | " +
                                @"Select-Object TaskName, Author, Triggers,Description, Date, State | " +
                                @"ConvertTo-Json" +
                            @"}";
            try
            {
                string output = "";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = powerShellCommand,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    }
                };
                process.Start();
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    // Handle non-zero exit code, which indicates an error occurred
                    throw new Exception();
                }
                while (!process.StandardOutput.EndOfStream)
                {
                    string? line = process.StandardOutput.ReadLine();
                    output += line + "\n";
                }
                List<ScheduledTask>? scheduledTasks = JsonConvert.DeserializeObject<List<ScheduledTask>>(output);
                if (scheduledTasks != null)
                {
                    foreach (var task in scheduledTasks)
                    {
                        task.GetStateString();
                    }
                    return scheduledTasks.ToArray();
                }
                return Array.Empty<ScheduledTask>();

            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching scheduled tasks. ", ex);
            }
        }
    }
}
