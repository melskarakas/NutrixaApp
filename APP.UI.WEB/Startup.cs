using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using APP.UI.WEB.BaseClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace APP.UI.WEB
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
            services.AddControllersWithViews();

            services.AddScoped<BaseClasses.OturumKontrol>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // OZ

            services.AddLocalization(options =>
            {
                // Resource (kaynak) dosyalarýmýzý ana dizin altýnda "Resources" klasorü içerisinde tutacaðýmýzý belirtiyoruz.
                options.ResourcesPath = "Resources";
            });


            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    // OZ
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(300));
            services.AddControllersWithViews().AddViewLocalization().AddRazorRuntimeCompilation(); // OZ : https://stackoverflow.com/questions/53639969/net-core-mvc-page-not-refreshing-after-changes/57637903#57637903

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("tr-TR");

                var cultures = new CultureInfo[]
                {
                new CultureInfo("tr-TR"),
                new CultureInfo("en-US")
                };

                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            app.UseRequestLocalization();
            // Destek vermemizi istediðimiz dilleri tutan bir liste oluþturuyoruz.
            var supportedCultures = new List<CultureInfo>{
                 new CultureInfo("tr-TR"),
                 new CultureInfo("en-US"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                DefaultRequestCulture = new RequestCulture("tr-TR")
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseCookiePolicy(); // OZ
            app.UseSession(); // OZ
            Operations.Configure(httpContextAccessor);


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
