using AspNetCoreRateLimit;
using CD.Common;
using CD.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDbCache;
using RewriteRules;
using System;
using VoltTools.Controllers;
using VoltTools.Models;
using VoltTools.Models.Views;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region ip limit rating
            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMongoDbCache(options =>
            {
                options.ConnectionString = "mongodb+srv://volttoolsadmin:voltadmin<secret98.a@volttools.uonol.mongodb.net/volttools?retryWrites=true&w=majority";
                options.DatabaseName = "volttools";
                options.CollectionName = "appcache";
                options.ExpiredScanInterval = TimeSpan.FromMinutes(10);
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configure ip rate limiting middleware
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            services.AddMvc();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion

            services.AddSingleton<IDatabase>(new Mongo());
            services.AddSingleton<ShortUrlAccounter>();
            services.AddSingleton<EmailSender>();
            services.AddSingleton<CalendarScheduler>();
            services.AddTransient<BaseView>();
            services.AddTransient<PageView>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandlerMiddleware("/error.html");

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRewriteMiddleware();

            app.UseStaticFiles();

            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthorization();

            app.ApplicationServices.GetService<CalendarScheduler>().StartSchedular();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Page}/{urltext?}");
            });
        }
    }
}
