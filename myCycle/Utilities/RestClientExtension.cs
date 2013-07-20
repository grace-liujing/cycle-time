using RestSharp;
using myCycle.Controllers;

namespace myCycle.Utilities
{
    public static class RestClientExtension
    {
        public static dynamic ExecuteXml(this IRestClient restClient , IRestRequest request)
        {
            var restResponse = restClient.Execute(request);
            return DynamicXml.Parse(restResponse.Content);
        }
    }
}