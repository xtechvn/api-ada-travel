using Caching.RedisWorker;
using Entities.ConfigModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REPOSITORIES.IRepositories;
using REPOSITORIES.Repositories.Login;
using System;

namespace API_LOGIN
{
    public class Startup
    {
        private readonly IConfiguration Configuration;//
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            // khoi tao lan dau tien chuoi config khi ung dung duoc chay.
            // no chi die khi ung ung die
            // Get config to instance model
            services.Configure<DataBaseConfig>(Configuration.GetSection("DataBaseConfig"));

            // Register services   
            services.AddSingleton(Configuration);
            
            services.AddSingleton<IUserCoreRepository, UserCoreRepository>();

            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            // Set session
            services.AddDistributedMemoryCache();

            // Setting Redis                     
            services.AddSingleton<RedisConn>();

            services.AddSession(option =>
            {
                // Set a short timeout for easy testing.
                option.IdleTimeout = TimeSpan.FromDays(1);
                option.Cookie.HttpOnly = true;
                // Make the session cookie essential
                option.Cookie.IsEssential = true;
            });


            services.AddCors(o => o.AddPolicy("MyApi", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RedisConn redisService)
        {
            //app.Run(context => {
            //    return context.Response.WriteAsync("Hello Readers!");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }          

            app.UseHttpsRedirection();
            //app.UseAntiXssMiddleware();
            app.UseRouting();

            // Inject the authorization middleware into the Request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            //Redis conn Call the connect method
            redisService.Connect();
            app.UseCors("MyApi");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
