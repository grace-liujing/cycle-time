using Autofac;
using Autofac.Integration.Mvc;

namespace myCycle.Controllers
{
    public class DomainModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<CardFactory>().Named<ICardFactory>("imp").InstancePerHttpRequest();
            builder.Register(c =>
                                 {
                                     var cardFactory = c.ResolveNamed<ICardFactory>("imp");
                                     return new CachedCardFactory(() => cardFactory);
                                 }).As<ICardFactory>();
        }
    }
}