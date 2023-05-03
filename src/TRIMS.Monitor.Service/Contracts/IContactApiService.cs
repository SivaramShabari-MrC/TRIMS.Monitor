using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Service
{
    public interface IContactApiService
    {
        public Task<GraphApiBatchResponseContacts> GetContacts(string[] emailIds);
        public Task<GraphApiBatchResponsePhotos> GetPhotos(string[] emailIds);
        public Task<string> GetPhoto(string emailId);
    }
}
