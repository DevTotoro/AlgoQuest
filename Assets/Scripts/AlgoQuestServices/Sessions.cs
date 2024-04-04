using System.Threading.Tasks;
using UnityEngine;

namespace AlgoQuestServices
{
    public static class Sessions
    {
        public struct CreateSessionRequestPayload
        {
            public string username;
        }
        
        public struct CreateSessionResponsePayload
        {
            public string id;
            public string username;
        }

        public static async Task<HttpResponse<CreateSessionResponsePayload>> Create(CreateSessionRequestPayload data)
        {
            var res = await Http.Post<CreateSessionResponsePayload, CreateSessionRequestPayload>("sessions", data);

            if (res.Success) Http.SessionId = res.Data.id;
            
            return res;
        }
    }
}
