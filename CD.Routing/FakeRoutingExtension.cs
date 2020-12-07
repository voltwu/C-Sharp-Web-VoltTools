using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using RewriteRules;

namespace CD.Routing
{
    public static class RewriteMiddlewareExtension
    {
        public static void UseRewriteMiddleware(this IApplicationBuilder app) {
            using (StreamReader apacheModRewriteStreamReader =
                File.OpenText("ApacheModRewrite.txt"))
            {
                var options = new RewriteOptions()
                    .AddApacheModRewrite(apacheModRewriteStreamReader);

                app.UseRewriter(options);
            }
        }
    }
}
