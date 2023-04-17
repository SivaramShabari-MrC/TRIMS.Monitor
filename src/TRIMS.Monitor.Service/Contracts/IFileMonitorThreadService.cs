﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Entity.API;

namespace TRIMS.Monitor.Service
{
    public interface IFileMonitorThreadService
    {
        public Task<FileMonitorThread[]?> GetMonitorThreads(string configFilePath, bool includeFiles, FolderType? folder);
        public FileDetail[] GetFilesFromFolder(string filePath);
        public Task<byte[]> DownloadFile(string configFilePath, string threadName, FolderType folder, string fileName);
        public Task MoveFile(string sourcePath, string destinationPath, string fileName);
        public Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus(string serverName, string fmsServiceName, string bfmsServicenName);
        public Task ExecuteWindowsServiceAction(string serverName, string serviceName, FMSWindowsServiceCommand commandType);


    }
}
