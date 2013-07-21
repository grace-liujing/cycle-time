using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using RestSharp;
using myCycle.Utilities;

namespace myCycle.Controllers
{
    public interface ICardFactory
    {
        Card Get(string projectName, string cardNumber);
        Card Get(string projectName, string cardNumber, int version);
    }

    public class CachedCardFactory : ICardFactory
    {
        private readonly Func<ICardFactory> proxy;
        public CachedCardFactory(Func<ICardFactory> proxy)
        {
            this.proxy = proxy;
        }

        public Card Get(string projectName, string cardNumber)
        {
            return Get(projectName, cardNumber, 10000);
        }

        public Card Get(string projectName, string cardNumber, int version)
        {
            var cardKey = GetKey(projectName, cardNumber, version);
            if (HttpContext.Current.Session[cardKey] == null)
            {
                HttpContext.Current.Session[cardKey] = proxy().Get(projectName, cardNumber, version);
            }
            return (Card)HttpContext.Current.Session[cardKey];
        }

        private static string GetKey(string projectName, string cardNumber, int version)
        {
            return string.Format("{0}+{1}+{2}", projectName, cardNumber, version);
        }
    }

    public class CardFactory : ICardFactory
    {
        private readonly IRestClient client;

        public CardFactory(IRestClient client)
        {
            this.client = client;
        }

        public Card Get(string projectName, string cardNumber)
        {
            return Get(projectName, cardNumber, 10000);
        }

        public Card Get(string projectName, string cardNumber, int version)
        {
            var versionedRequest = new RestRequest("projects/{project}/cards/{card}.xml?version={version}");
            versionedRequest.AddUrlSegment("project", projectName);
            versionedRequest.AddUrlSegment("card", cardNumber);
            versionedRequest.AddUrlSegment("version", version.ToString(CultureInfo.InvariantCulture));
            return Card.Create(client.ExecuteXml(versionedRequest));
        }
    }
}