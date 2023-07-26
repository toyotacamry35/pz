using SharedCode.EntitySystem;
using System;

namespace SharedCode.Utils.HttpServer
{
    public class HttpServerFactory
    {
        public static Func<IEntitiesRepository, IHttpServer> CreateDelegate;

        public static IHttpServer CreateServer(IEntitiesRepository repository)
        {
            if(CreateDelegate == null)
                throw new InvalidOperationException($"{typeof(HttpServerFactory).Name} is not initialized");

            return CreateDelegate(repository);
        }
    }
}
