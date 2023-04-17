using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRIMS.Monitor.Entity
{
    public class FileDetail
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.MinValue;
    }
}
