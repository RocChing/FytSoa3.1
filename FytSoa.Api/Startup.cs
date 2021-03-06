using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Service;
using FytSoa.Service.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;

namespace FytSoa.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            FytSoaConfig.InitConfig(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHttpClient();
            services.RegisterAssembly("FytSoa.Service");
            services.AddTransient(typeof(IBaseService<>), typeof(BaseService<>));

            string allowedCors = Configuration.GetValue<string>("AllowedCors");
            services.AddCors(options => options.AddPolicy("MyCorsPolicy",
               builder =>
               {
                   builder.AllowAnyMethod() //允许任意请求方式
                          .AllowAnyHeader() //允许任意header
                          .AllowCredentials()//允许验证http://127.0.0.1
                          .WithOrigins(allowedCors.Split(','));
                   //.SetIsOriginAllowed(_ => true); //指定特定域名才能访问
               }));

            services.AddSignalR();
            services.AddRazorPages();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FytSoa API", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var apiXmlPath = Path.Combine(basePath, "FytSoa.Api.xml");
                var coreXmlPath = Path.Combine(basePath, "FytSoa.Core.xml");
                c.IncludeXmlComments(apiXmlPath);
                c.IncludeXmlComments(coreXmlPath);
            });

            services.AddHostedService<SongHubBackgroudService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //加载配置文件
            NLog.LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            app.UseStaticFiles();
            app.UseCors("MyCorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FytSoa API V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<SongHub>("/songHub");
            });
        }
    }
}
