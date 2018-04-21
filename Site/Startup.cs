using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Code.Download;
using Scraper.Code.Ranking;
using Scraper.Code.SearchResultUrlExtraction;

namespace Site
{
    public class Startup
    {
        // The prefix which Razor views will need to prepend before paths to static files.
        public static string StaticFilesRequestPath { get; private set; }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Hook up our interfaces for DI. These all need to be scoped as HttpDownloader has
            // request-specific state.
            services.AddScoped<IDownloader, HttpDownloader>();
            services.AddScoped<IExtractor, GoogleExtractor>();
            services.AddScoped<IRanker, ExtractionRanker>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            string url_prefix = Configuration["UrlPrefix"];
            string no_trailing_slash = url_prefix.EndsWith("/") ? url_prefix.Substring(0, url_prefix.Length - 1) : url_prefix;
            StaticFilesRequestPath = "/" + no_trailing_slash;
            app.UseStaticFiles(StaticFilesRequestPath);

            app.UseMvc(routes => routes.MapRoute("default",  url_prefix + "{controller=Home}/{action=Index}"));
        }
    }
}
