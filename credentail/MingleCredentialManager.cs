using System.Net;
using RestSharp;

namespace credentail
{
    public class MingleCredentialManager : ICredentialManager
    {
        private readonly IRestClient client;

        public MingleCredentialManager(IRestClient client)
        {
            this.client = client;
        }

        public bool Check(string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("projects.xml");
            var restResponse = client.Execute(request);
            return restResponse.StatusCode == HttpStatusCode.OK;
        }
    }
}