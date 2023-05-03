using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Service
{
    public class ContactApiService : IContactApiService
    {
        private readonly string baseUrl;
        private readonly string authHeader;
        public ContactApiService(IConfiguration config)
        {
            baseUrl = config["MiscrosoftGraphApi:Url"]!;
            authHeader = "Bearer " + config["MiscrosoftGraphApi:AccessToken"];
        }
        public async Task<GraphApiBatchResponseContacts> GetContacts(string[] emailIds)
        {
            string api = baseUrl + "/$batch";
            List<GraphApiBatchRequest> graphApiBatchRequests = new();
            foreach (string emailId in emailIds)
            {
                graphApiBatchRequests.Add(new GraphApiBatchRequest
                {
                    Id = emailId,
                    Url = $"/users/{emailId}",
                    Method = "GET"
                });
            }
            GraphApiBatchRequestBody body = new() { Requests = graphApiBatchRequests };

            GraphApiBatchResponseContacts contacts = await api
                .WithHeader("Authorization", authHeader)
                .PostJsonAsync(body)
                .ReceiveJson<GraphApiBatchResponseContacts>();
            return contacts;
        }

        public async Task<GraphApiBatchResponsePhotos> GetPhotos(string[] emailIds)
        {
            string api = baseUrl + "/$batch";
            List<GraphApiBatchRequest> graphApiBatchRequests = new();
            foreach (string emailId in emailIds)
            {
                graphApiBatchRequests.Add(new GraphApiBatchRequest
                {
                    Id = emailId,
                    Url = $"/users/{emailId}/photo/$value",
                    Method = "GET"
                });
            }
            GraphApiBatchRequestBody body = new() { Requests = graphApiBatchRequests };
            GraphApiBatchResponsePhotos photos = await api
                .WithHeader("Authorization", authHeader)
                .PostJsonAsync(body)
                .ReceiveJson<GraphApiBatchResponsePhotos>();
            return photos;
        }

        public async Task<string> GetPhoto(string emailId)
        {
            string api = baseUrl + $"/users/{emailId}/photo/$value";
            var result = await api
                .WithHeaders(new
                {
                    Authorization = authHeader,
                    Content_Type = "image/jpeg"
                })
                .GetAsync();
            byte[] base64 = await result.GetBytesAsync();
            string photo = Convert.ToBase64String(base64);
            return photo;
        }

    }
}
