using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Manager
{
    public interface ITransactionManager
    {
        public Task<List<TransactionReport>> GetTransactionReport(DateTime from, DateTime to);
    }
}
