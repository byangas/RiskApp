using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RiskApp.Data;
using RiskApp.Repositories;
using RiskApp.Services;
using RiskApp.Utility;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiskApp
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "RMAUTH2";
                options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/SignIn");
                // this is so that when client calls API and user is not authenticated, we don't get redirected 
                // to Login screen. Redirects are not for the API, but for the "View" controllers.
                options.Events.OnRedirectToLogin = (context) =>
                 {
                     if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                     {
                         context.Response.StatusCode = 401;
                     }
                     else
                     {
                         context.Response.Redirect(context.RedirectUri);
                     }
                     return Task.CompletedTask;
                 };

            });

            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            } );

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<EmailRenderEngine>();

            //TODO: needed for storing connection string in ENV instead of in appsettings file
            //IConfigurationSection configurationSection = Configuration.get("bob");
            // object v = configurationSection.GetValue(typeof(string), "AlsoBob");
            // also could doe :  Configuration.get("ConnectionStrings:RiskAppDb")

            // adding this as "scoped" means that all http requests
            services.AddScoped<ConnectionManager, ConnectionManager>();
            // repos
            services.AddScoped<UserAccountRepository>();
            services.AddScoped<RegistrationRepository>();
            services.AddScoped<ApplicationRoleRepository>();
            services.AddScoped<ProfileRepository>();
            services.AddScoped<ProfileContactRepository>();
            services.AddScoped<CompanyRepository>();
            services.AddScoped<ContactRepository>();
            services.AddScoped<CustomerRepository>();
            services.AddScoped<PolicyRepository>();
            services.AddScoped<MessageRepository>();
            services.AddScoped<CarrierAppointmentRepository>();

            // services
            services.AddScoped<ContactService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<ProfileService>();
            services.AddScoped<MessageService>();
            services.AddScoped<UserAccountService>();
            services.AddScoped<RegistrationService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<PolicyService>();
            services.AddScoped<MessageService>();
            services.AddScoped<CarrierAppointmentService>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

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
