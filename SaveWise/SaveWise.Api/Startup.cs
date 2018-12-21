using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SaveWise.Api.Common;
using SaveWise.Api.Extensions;
using SaveWise.BusinessLogic.Common;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;
using SaveWise.DataLayer.User;

namespace SaveWise.Api
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
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

            var securitySettings = Configuration.GetSection("SecuritySettings");
            services.Configure<SecuritySettings>(securitySettings);
            
            var secret = Encoding.ASCII.GetBytes(securitySettings.Get<SecuritySettings>().Secret);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt =>
                {
                    jwt.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var userId = context.Principal.Identity.Name;
                            var user = userService.GetById(userId);
                            if (user == null)
                            {
                                context.Fail("Unauthorized");
                            }

                            return Task.CompletedTask;
                        }
                    };
                    jwt.RequireHttpsMetadata = false;
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secret),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            IConfigurationSection connectionStringSection =
                Configuration.GetSection("MongoConnection:ConnectionString");
            
            IConfigurationSection databaseSection = Configuration.GetSection("MongoConnection:Database");

            
            var predefinedCategories = Configuration.GetSection("PredefinedCategories").Get<PredefinedCategories>();

            services.AddSingleton(predefinedCategories);
            
            services.AddSingleton<IIdentityProvider, IdentityProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISaveWiseContext>(new SaveWiseContext(
                connectionStringSection.Value,
                databaseSection.Value));
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRepositoryFactory, RepositoryFactory>();
            services.AddTransient<IExpenseService, ExpenseService>();
            services.AddTransient<IPlanService, PlanService>();
            services.AddTransient<IService<ExpenseCategory>, Service<ExpenseCategory>>();
            services.AddTransient<IIncomeCategoryService, IncomeCategoryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options => options.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition"));
            app.ConfigureExceptionHandler();

            app.UseAuthentication();
            
            app.UseMvc();
        }
    }
}