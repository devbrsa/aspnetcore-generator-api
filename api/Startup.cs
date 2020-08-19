using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace api
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
            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                })
                .AddXmlSerializerFormatters();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NEW2 Generate Random Data API", Version = "v1" });
            });

            services.Configure<MailServerConfig>(Configuration.GetSection("mailserver"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IOptions<MailServerConfig> mailServerConfigAccessor)
        {
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Generate Random Data API V1");
            });

            var redirectRootToSwagger = new RewriteOptions()
                .AddRedirect("^$", "swagger");
            app.UseRewriter(redirectRootToSwagger);

            var mailServerConfig = mailServerConfigAccessor.Value;
            Console.WriteLine($"Mail server: {mailServerConfig.Host}:{mailServerConfig.Port}");
        }
    }
}