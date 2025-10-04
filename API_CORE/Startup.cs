using Caching.RedisWorker;
using Entities.ConfigModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Repositories.IRepositories;
using Repositories.Repositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.BankingAccount;
using REPOSITORIES.IRepositories.Clients;
using REPOSITORIES.IRepositories.Comment;
using REPOSITORIES.IRepositories.Contract;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Fly;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.IRepositories.IReportRevenue;
using REPOSITORIES.IRepositories.Notify;
using REPOSITORIES.IRepositories.VinWonder;
using REPOSITORIES.Repositories;
using REPOSITORIES.Repositories.BankingAccount;
using REPOSITORIES.Repositories.Clients;
using REPOSITORIES.Repositories.Comment;
using REPOSITORIES.Repositories.Elasticsearch;
using REPOSITORIES.Repositories.Fly;
using REPOSITORIES.Repositories.Hotel;
using REPOSITORIES.Repositories.Notify;
using REPOSITORIES.Repositories.ReportRevenue;
using REPOSITORIES.Repositories.VinWonder;
using System;
using System.Text;
namespace API_CORE
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
            services.AddSingleton<IServicePiceRepository, ServicePiceRepository>();
            services.AddSingleton<ICampaignRepository, CampaignRepository>();
            services.AddSingleton<IProductFlyTicketServiceRepository, ProductFlyTicketServiceRepository>();
            services.AddSingleton<IAllCodeRepository, AllCodeRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoomFunRepository, RoomFunRepository>();
            services.AddTransient<ITelegramRepository, TelegramRepository>();
            services.AddTransient<IGroupClassAirlinesDetailRepository, GroupClassAirlinesDetailRepository>();
            services.AddTransient<IGroupClassAirlinesRepository, GroupClassAirlinesRepository>();
            services.AddTransient<IAirlinesRepository, AirlinesRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IBankOnePayRepository, BankOnePayRepository>();
            services.AddTransient<IAccountB2CRepository,AccountB2CRepository>();
            services.AddTransient<IContactClientRepository, ContactClientRepository>();
            services.AddTransient<IFlyBookingDetailRepository, FlyBookingDetailRepository>();
            services.AddTransient<IFlightSegmentRepository, FlightSegmentRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IBagageRepository, BagageRepository>();
            services.AddTransient<IPassengerRepository, PassengerRepository>();
            services.AddTransient<IAirPortCodeRepository, AirPortCodeRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IAttachFileRepository, AttachFileRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IDepositHistoryRepository, DepositHistoryRepository>();
            services.AddTransient<IAllotmentFundRepository, AllotmentFundRepository>();
            services.AddTransient<IElasticsearchDataRepository, ElasticsearchDataRepository>();
            services.AddTransient<IAccountClientRepository, AccountClientRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IGroupProductRepository, GroupProductRepository>();
            services.AddTransient<REPOSITORIES.Repositories.IdentifierServiceRepository, REPOSITORIES.Repositories.IdentifierServiceRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IContractPayRepository, ContractPayRepository>();
            services.AddTransient<IHotelDetailRepository, HotelDetailRepository>();
            services.AddTransient<IContractRepository, ContractRepository>();
            services.AddTransient<IVoucherRepository, VoucherRepository>();
            services.AddTransient<IAccountB2BRepository, AccountB2BRepository>();
            services.AddTransient<IPlayGroundDetailRepository, PlayGroundDetailRepository>();
            services.AddTransient<INotifyRepository, NotifyRepository>();
            services.AddTransient<ICommentRepository,CommentRepository>();
            services.AddTransient<IRecruitmentRepository, RecruitmentRepository>();


            services.AddTransient<IOtherBookingRepository, OtherBookingRepository>();
            services.AddTransient<ITourRepository, TourRepository>();
            services.AddTransient<IAllCodeRepository, AllCodeRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IHotelBookingRepositories, HotelBookingRepositories>();

            services.AddTransient<IBankingAccountRepository, BankingAccountRepository>();

            services.AddTransient<IReportRevenueRepository, ReportRevenueRepository>();
            services.AddSingleton<ILocationRepository, LocationRepository>();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<IHotelBookingRoomExtraPackageRepository, HotelBookingRoomExtraPackageRepository>();
            services.AddSingleton<IHotelBookingRoomRepository, HotelBookingRoomsRepository>();
            services.AddSingleton<IDebtGuaranteeRepository, DebtGuaranteeRepository>();
            services.AddSingleton<ISaveBookingRepository, SaveBookingRepository>();
            services.AddTransient<IIdentifierServiceRepository, IdentifierServiceRepository>();
            services.AddSwaggerGen();

            // Mongo DB                        
            services.AddSingleton<IFlyBookingMongoRepository, FlyBookingMongoRepository>();
            services.AddSingleton<IHotelBookingMongoRepository, HotelBookingMongoRepository>();
            services.AddSingleton<IVinWonderBookingRepository, VinWonderBookingRepository>();

            // services.AddMvc().AddNewtonsoftJson();
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

            // services.AddResponseCaching();

            //Configure authorization middleware in the startup configureService method.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

           
           
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
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Adavigo API Document");
                c.RoutePrefix = "doc";
            });

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
