using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Entity.API;

namespace TRIMS.Monitor.Service
{
    public class FileMonitorThreadService : IFileMonitorThreadService
    {
        private readonly ILogger<FileMonitorThreadService> _logger;
        public FileMonitorThreadService(ILogger<FileMonitorThreadService> logger)
        {
            _logger = logger;
        }
        public FileMonitorThread[]? GetMonitorThreads(string configFilePath, FolderType? folder)
        {
            string configFileXML = File.ReadAllText(configFilePath);
            XmlDocument configFileDocument = new();
            configFileDocument.LoadXml(configFileXML);
            string monitorsJson = JsonConvert.SerializeXmlNode(configFileDocument);
            ConfigFile? configFileJson = JsonConvert.DeserializeObject<ConfigFile>(monitorsJson);
            var fileMonitorThreads = configFileJson?.Configuration?.FileMonitorSection?.Monitors?.AddMonitor;
            return fileMonitorThreads;
        }

        public FileDetail[] GetFilesFromFolder(string filePath)
        {
            if (filePath == "") return Array.Empty<FileDetail>();
            IList<FileDetail> result = new List<FileDetail>(); ;
            StringBuilder builder = new(filePath);
            if (builder.ToString().Contains("C:")) builder.Replace("C:", @"\\crctappsdev01"); //replace C: with dev endpoint
            var path = builder.ToString();
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                foreach (var file in files)
                {
                    result.Add(new FileDetail()
                    {
                        Name = file.Name,
                        Date = file.CreationTime,
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting files. Directory path: '{filePath}'. Exception: {e}");
                //throw new Exception($"Error getting files. Directory path: {filePath}. Exception: {e}");
            }
            return result.ToArray();
        }

        public byte[] DownloadFile(string configFilePath, string threadName, FolderType folder, string fileName)
        {
            try
            {
                var fileMonitorThreads = GetMonitorThreads(configFilePath, null);
                var folderPath = GetFolderPath(fileMonitorThreads, threadName, folder);
                FileStream fileStream = new FileStream($@"{folderPath}/{fileName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var memStream = new MemoryStream();
                fileStream.CopyTo(memStream);
                return memStream.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Error while downloading file {fileName} from thread {threadName}. Exception: {e}");
            }
        }

        public void MoveFile(string sourcePath, string destinationPath, string fileName)
        {
            try
            {
                // Check if source file exists
                var source = Path.Combine(sourcePath, fileName);
                if (!File.Exists(source))
                    throw new FileNotFoundException($"Source file {fileName} not found at {sourcePath}");

                // Check if destination file already exists
                var destination = Path.Combine(destinationPath, fileName);
                if (File.Exists(destination))
                    throw new IOException($"Destination file {fileName} already exists at {destinationPath}");

                // Move the file to the destination
                File.Move(source, destination);
            }
            catch (FileNotFoundException e)
            {
                throw new Exception($"Error while moving file {fileName} from {sourcePath} to {destinationPath}. {e.Message}");
            }
            catch (IOException e)
            {
                throw new Exception($"Error while moving file {fileName} from {sourcePath} to {destinationPath}. {e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Error while moving file {fileName} from {sourcePath} to {destinationPath}. {e.Message}");
            }
        }

        public async Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus(string serverName, string fmsServiceName, string bfmsServiceName)
        {
            string powerShellCommand = $"/C invoke-command -ComputerName {serverName} -ScriptBlock " +
                                         " { " +
                                            @$" Get-Service -Name ""{fmsServiceName}"" | Select-Object Name, Status ;" +
                                            @$" Get-Service -Name ""{bfmsServiceName}"" | Select-Object Name, Status ;" +
                                         " } | ConvertTo-Json ";
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
                List<ServiceInfo>? serviceInfoList = JsonConvert.DeserializeObject<List<ServiceInfo>>(output);

                string FMS = serviceInfoList?.Where(x => x.Name == fmsServiceName).FirstOrDefault()!.Status?.Value!;
                string BFMS = serviceInfoList?.Where(x => x.Name == bfmsServiceName).FirstOrDefault()!.Status?.Value!;
                return new FMSWindowsServiceStatus { FMS = FMS, BFMS = BFMS };
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching FMS and BFMS statuses. ", ex);
            }
        }


        public async Task ExecuteWindowsServiceAction(string serverName, string serviceName, FMSWindowsServiceCommand commandType)
        {
            string serverScript;
            if (commandType == FMSWindowsServiceCommand.Stop) serverScript = GetStopServiceCommand(serverName, serviceName);
            else serverScript = StartServiceCommand(serverName, serviceName);
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = serverScript,
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
                    string output = "";
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string? line = process.StandardOutput.ReadLine();
                        output += line + "\n";
                    }
                    throw new Exception("Error executing powershell script: " + output);
                }
                return;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while executing {commandType} for {serviceName} in {serverName}", ex);
            }
        }

        private string GetFolderPath(FileMonitorThread[]? fileMonitorThreads, string threadName, FolderType folder)
        {
            if (fileMonitorThreads == null) return string.Empty;
            if (!fileMonitorThreads.Any()) return string.Empty;

            string folderPath = string.Empty;
            var thread = fileMonitorThreads?.Where(thread => thread.ThreadName == threadName).FirstOrDefault();

            if (folder == FolderType.ProcessedFolder)
                folderPath = thread?.Endpoint?.AddEndPoint?[0].ProcessedFolder ?? thread?.Endpoint?.AddEndPoint?[1].ProcessedFolder ?? "";
            else if (folder == FolderType.ErrorsFolder)
                folderPath = thread?.Endpoint?.AddEndPoint?[0].ErrorsFolder ?? thread?.Endpoint?.AddEndPoint?[1].ErrorsFolder ?? "";
            else if (folder == FolderType.DestinationFolder)
                folderPath = thread?.Endpoint?.AddEndPoint?[0].DestinationFolder ?? thread?.Endpoint?.AddEndPoint?[1].DestinationFolder ?? "";
            else if (folder == FolderType.DebugFolder)
            {
                folderPath = thread?.Endpoint?.AddEndPoint?[0].SourceFolder ?? thread?.Endpoint?.AddEndPoint?[1].SourceFolder ?? "";
                if (folderPath != "") folderPath += "\\Debug";
            }
            else
                folderPath = thread?.Endpoint?.AddEndPoint?[0].SourceFolder ?? thread?.Endpoint?.AddEndPoint?[1].SourceFolder ?? "";

            return folderPath;
        }

        private static string GetStopServiceCommand(string serverName, string serviceName)
        {
            return $"/C invoke-command -ComputerName {serverName} -ScriptBlock {{ " +
                                $"$serviceName = '{serviceName}' ; " +
                                @"$processid = (Get-WmiObject Win32_Service | Where-Object {$_.Name -eq $serviceName}).ProcessId; " +
                                @"taskkill /F /PID $processid; " +
                                @"}";
        }

        private static string StartServiceCommand(string serverName, string serviceName)
        {
            return $"/C invoke-command -ComputerName {serverName} -ScriptBlock {{ Start-Service -Name '{serviceName}' }}";
        }

        private class ServiceInfo
        {
            public string Name { get; set; } = string.Empty;
            public StatusInfo? Status { get; set; }
            public string PSComputerName { get; set; } = string.Empty;
            public string RunspaceId { get; set; } = string.Empty;
            public bool PSShowComputerName { get; set; }
        }

        private class StatusInfo
        {
            public int value { get; set; }
            public string Value { get; set; } = string.Empty;
        }

    }

}
