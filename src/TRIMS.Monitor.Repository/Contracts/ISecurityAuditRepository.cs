using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Repository
{
    public interface ISecurityAuditRepository
    {
        public Task LogAudit(SecurityAudit securityAudit);
    }
}
