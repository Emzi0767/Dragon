using Dragon.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RiotPls.DataDragon;
using RiotPls.DataDragon.Enums;

namespace Dragon
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
            services.AddHttpClient();
            
            services.AddSingleton(x => new DataDragonClientOptions
            {
                CacheMode = CacheMode.KeepAll,
                DefaultLanguage = Language.AmericanEnglish
            });
            services.AddSingleton<DataDragonClient>();
            services.AddSingleton<DataDragonService>();

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            services.AddSession(options =>
            {
                options.Cookie.Name = "dragon";
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
            });

            services.AddControllersWithViews();
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/home/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseSession()
                .UseRouting()
                .UseEndpoints(endpoints =>
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}