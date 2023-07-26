using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratedCode.Custom.Config;
using System.Threading;

namespace SharedCode.Utils.HttpServer
{
    public interface IHttpServer
    {
        Task StartAsync(RestApiServiceEntityConfig config, CancellationToken ct);
    }
}
