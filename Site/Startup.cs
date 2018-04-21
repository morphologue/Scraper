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

            app.UseStaticFiles();

            app.UseMvc(routes => routes.MapRoute("default",  Configuration["UrlPrefix"] + "{controller=Home}/{action=Index}"));
        }
    }
}
