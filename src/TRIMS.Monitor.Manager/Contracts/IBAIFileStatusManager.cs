using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Repository;

namespace TRIMS.Monitor.Manager
{
    public interface IBAIFileStatusManager
    {
        public Task<BAIFileStatusResponse[]> CheckFileForDate(EnvironmentType environment, DateTime date);
    }
}
