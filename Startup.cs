using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAppIISNginx.Middleware;

namespace WebAppIISNginx
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("进入第一个委托 执行下一个委托之前\r\n");
            //    //调用管道中的下一个委托
            //    await next.Invoke();
            //    await context.Response.WriteAsync("结束第一个委托 执行下一个委托之后\r\n");
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("进入第二个委托\r\n");
            //    await context.Response.WriteAsync("Hello from 2nd delegate. \r\n");
            //    await context.Response.WriteAsync("结束第二个委托\r\n");
            //});

            //app.Map("/map1", HandleMapTest1);

            //app.Map("/map2", HandleMapTest2);

            //app.MapWhen(context => context.Request.Query.ContainsKey("branch"),
            //    HandleBranch);

            //app.Use((context, next) =>
            //{
            //    var cultureQuery = context.Request.Query["culture"];
            //    if (!string.IsNullOrWhiteSpace(cultureQuery))
            //    {
            //        var culture = new CultureInfo(cultureQuery);

            //        CultureInfo.CurrentCulture = culture;
            //        CultureInfo.CurrentUICulture = culture;
            //    }

            //    // Call the next delegate/middleware in the pipeline
            //    return next();
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(
            //        $"Hello {CultureInfo.CurrentCulture.DisplayName}");
            //});


            //app.Map("/level1", level1App => {
            //    level1App.Map("/level2a", level2AApp => {
            //        // "/level1/level2a"
            //        app.Run(async context =>
            //        {
            //            await context.Response.WriteAsync("/level1/level2a");
            //        });
            //    });
            //    level1App.Map("/level2b", level2BApp => {
            //        // "/level1/level2b"
            //        app.Run(async context =>
            //        {
            //            await context.Response.WriteAsync("/level1/level2b");
            //        });
            //    });
            //});

            //app.UseRequestCulture();
            //app.UseMyMiddleware();
            app.UseRequestCacheView();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(
            //        $"Hello {CultureInfo.CurrentCulture.DisplayName}");
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello from non-Map delegate. <p>");
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1");
            });
        }

        private static void HandleMapTest2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 2");
            });
        }

        private static void HandleBranch(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var branchVer = context.Request.Query["branch"];
                await context.Response.WriteAsync($"Branch used = {branchVer}");
            });
        }
    }
}
