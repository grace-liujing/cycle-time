using System.Web;
using Autofac;
using RestSharp;
using Autofac.Integration.Mvc;

namespace credentail
{
    public class CredentialModule : Module
    {
        private readonly string baseApiUrl;

        public CredentialModule(string baseApiUrl)
        {
            this.baseApiUrl = baseApiUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(
                c =>
                    {
                        if(HttpContext.Current.Session["RESTCLIENT"] == null)
                        {
                            HttpContext.Current.Session["RESTCLIENT"] = new RestClient(baseApiUrl);
                        }
                        return (RestClient)HttpContext.Current.Session["RESTCLIENT"];
                    }).As<IRestClient>().InstancePerHttpRequest();
            builder.RegisterType<MingleCredentialManager>().As<ICredentialManager>();
        }
    }
}