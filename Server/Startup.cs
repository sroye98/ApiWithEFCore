using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLogic.Entities;
using DataLogic.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BusinessLogic.Settings;
using BusinessLogic.Services;
using BusinessLogic.Interfaces;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseInternalServiceProvider(sp);

                if (env.IsDevelopment())
                {
                    options.UseInMemoryDatabase("testdb");
                }
                else
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                }
            });

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddCors(options => { });

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ValidatorActionFilter));
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            var appSettingsSection = Configuration.GetSection("SecuritySettings");
            services.Configure<SecuritySettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<SecuritySettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddHttpContextAccessor();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ICommunicationService, CommunicationService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
