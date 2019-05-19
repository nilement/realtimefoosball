using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ToughBattle.Database;
using ToughBattle.Facades;
using ToughBattle.Services;

namespace ToughBattle
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
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<FoosballContext>(options => options.UseSqlServer(Configuration.GetSection("Database:ConnectionString").Value));
            services.AddScoped<IStatisticsFacade, FakeStatisticsFacade>();
            services.AddScoped<ITournamentsFacade, TournamentsFacade>();
            services.AddScoped<IPlayersFacade, PlayersFacade>();
            services.AddScoped<IGameFacade, GameFacade>();
            services.AddScoped<IUserService, UserService>();
            services.AddHttpClient();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secret = Configuration.GetSection("Credentials:App:Secret").Value;
                    byte[] signingKey = Encoding.ASCII.GetBytes(secret);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(signingKey)
                    };
                });
            services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors(builder => builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod());

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller}/{action}");

                routes.MapRoute("Spa", "{*url}", defaults: new { controller = "Home", action = "Spa" });
            });

        }
    }
}
