using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;

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
            
            IConfigurationSection connectionStringSection =
                Configuration.GetSection("MongoConnection:ConnectionString");
            
            IConfigurationSection databaseSection = Configuration.GetSection("MongoConnection:Database");

            services.AddSingleton<ISaveWiseContext>(new SaveWiseContext(
                connectionStringSection.Value,
                databaseSection.Value));
            services.AddTransient<IRepositoryFactory, RepositoryFactory>();
            services.AddTransient<IExpenseService, ExpenseService>();
            services.AddTransient<IService<Plan>, PlanService>();
            services.AddTransient<IService<ExpenseCategory>, Service<ExpenseCategory>>();
            services.AddTransient<IService<IncomeCategory>, Service<IncomeCategory>>();
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
                .WithExposedHeaders("Content-Disposition"));
            
            app.UseMvc();
        }
    }
}