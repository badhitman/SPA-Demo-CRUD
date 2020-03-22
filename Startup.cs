using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Controllers;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.AuthorizePolicies;
using System;
using System.Security.Claims;

namespace SPADemoCRUD
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IConfiguration BuildConfiguration { get; set; }
        private AppConfig AppOptions { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            // создаем конфигурацию
            BuildConfiguration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDataBaseContext>(options => options.UseSqlServer(connection));

            #region AccessMinLevelHandler
            services.AddScoped<IAuthorizationHandler, AccessMinLevelHandler>();

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("AccessMinLevel" + AccessLevelUserRolesEnum.Auth.ToString(),
                    policy => policy.Requirements.Add(new AccessMinLevelRequirement(AccessLevelUserRolesEnum.Auth)));
                opts.AddPolicy("AccessMinLevel" + AccessLevelUserRolesEnum.Verified.ToString(),
                    policy => policy.Requirements.Add(new AccessMinLevelRequirement(AccessLevelUserRolesEnum.Verified)));
                opts.AddPolicy("AccessMinLevel" + AccessLevelUserRolesEnum.Privileged.ToString(),
                    policy => policy.Requirements.Add(new AccessMinLevelRequirement(AccessLevelUserRolesEnum.Privileged)));
                opts.AddPolicy("AccessMinLevel" + AccessLevelUserRolesEnum.Manager.ToString(),
                    policy => policy.Requirements.Add(new AccessMinLevelRequirement(AccessLevelUserRolesEnum.Manager)));
                opts.AddPolicy("AccessMinLevel" + AccessLevelUserRolesEnum.Admin.ToString(),
                    policy => policy.Requirements.Add(new AccessMinLevelRequirement(AccessLevelUserRolesEnum.Admin)));
                opts.AddPolicy("AccessMinLevel" + AccessLevelUserRolesEnum.ROOT.ToString(),
                    policy => policy.Requirements.Add(new AccessMinLevelRequirement(AccessLevelUserRolesEnum.ROOT)));
                //
                opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();
            });
            #endregion

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            // Настройка параметров и DI
            services.AddOptions();
            services.Configure<AppConfig>(BuildConfiguration);

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(AppOptions.SessionCookieExpiresSeconds);
                //
                options.Cookie.Name = "ApplicationCookie";
                options.Cookie.IsEssential = true;
            });
            services.AddMemoryCache();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => 
            { 
                options.LoginPath = new PathString("/signin/");
                options.AccessDeniedPath = new PathString("/accessdenied/");
                options.SlidingExpiration = true;
                options.LogoutPath = new PathString("/signin/");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                aSessionController.UpdateSession(context, AppOptions);
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
