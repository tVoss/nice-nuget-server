using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using NiceNuget.Api.Infrastructure;
using NiceNuget.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NiceNuget.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            AmazonS3Client s3Client = new AmazonS3Client(
                Configuration["Aws:Client"],
                Configuration["Aws:Secret"],
                RegionEndpoint.USEast2
            );

            services.AddSingleton(s3Client);
            services.AddSingleton<XmlFormatter>();
            services.AddSingleton<IPackageProvider, S3PackageProvider>();
            services.AddSingleton<IPackageCache, NaivePackageCache>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRefreshCacheMiddleware();
            app.UseMvc();
        }
    }
}
