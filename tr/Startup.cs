using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using tr.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace tr
{
    public class Startup
    {
        public string connexStr = "";
        public string jsonIn = null;
        public Startup(IConfiguration configuration,
            IWebHostEnvironment hostingEnv)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder();
            string crp = hostingEnv.ContentRootPath;

            builder
                .SetBasePath(crp);  // only in effect after build
            string strSettings = "";
            if (hostingEnv.IsDevelopment())
            {
                strSettings = "appsettings.Development.json";
            }
            else
            {
                strSettings = "appsettings.json";
            }

            string newPath = Path.GetFullPath(Path.Combine(crp, strSettings));  // to fix one odd configuration
            jsonIn = File.ReadAllText(newPath);
            Configuration = builder.Build();
            JObject appSettings = JObject.Parse(jsonIn);  // if any of this fails - it is best to let start up fail
            connexStr = appSettings.SelectToken("ConnectionStrings")?.SelectToken("SubjectDbConnection")?.Value<string>();
            if (string.IsNullOrWhiteSpace(connexStr))
                throw new Exception("The database connection string is not available at startup");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SubjectDbContext>(opt =>
               opt.UseSqlServer(connexStr));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
