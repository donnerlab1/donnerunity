using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Internal;

public class MacaroonCallCredentials
{
    public CallCredentials credentials;
    public MacaroonCallCredentials(string macaroon)
    {
        var asyncAuthInterceptor = new AsyncAuthInterceptor(async (context, metadata) =>
        {
            await Task.Delay(100).ConfigureAwait(false);
            metadata.Add("macaroon", macaroon);
        });
        credentials = CallCredentials.FromInterceptor(asyncAuthInterceptor);
    }
}


