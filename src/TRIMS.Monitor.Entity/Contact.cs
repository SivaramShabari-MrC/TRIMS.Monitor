using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRIMS.Monitor.Entity
{
    public class Contact
    {
        public string DisplayName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string OfficeLocation { get; set; } = string.Empty;
    }
    public class GraphApiBatchResponseContacts
    {
        public ContactResponse[]? Responses { get; set; }

    }

    public class GraphApiBatchResponsePhotos
    {
        public PhotoResponse[]? Responses { get; set; }

    }
    public class ContactResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Contact? Body { get; set; }
    }

    public class PhotoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class GraphApiBatchRequestBody
    {
        public List<GraphApiBatchRequest>? Requests { get; set; }
    }
    public class GraphApiBatchRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
