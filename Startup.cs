using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using TestAPI.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace TestAPI
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
            services.AddControllers()
                .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //dziêki DI bêdzi mo¿na u¿yæ w innej czêœci aplikacji
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            #region CORS
            var allowOrigins = Configuration.GetSection("CORS-Settings:Allow-Origins").Get<string[]>();
            services.AddCors(o =>
                o.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                    .WithOrigins(allowOrigins)
                    //.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
            ));
            #endregion

            #region Ustawienie po³¹czenia z baz¹ danych
            //Wersja dla MariaDB
            if (Environment.OSVersion.ToString().StartsWith("Unix"))
            {
                services.AddDbContext<AuthenticationContext>(options => options
                    .UseMySql(Configuration.GetConnectionString("MariaDB_Unix"), mySqlOptions => mySqlOptions
                        // replace with your Server Version and Type
                        .ServerVersion(new Version(10, 1, 44), ServerType.MariaDb)));
                #region Only helper logs
                Console.WriteLine("operating system Unix");
                Console.WriteLine(Environment.OSVersion.ToString());
                Console.WriteLine(Environment.UserName.ToString());
                Console.WriteLine(Environment.UserDomainName.ToString());
                Console.WriteLine(Environment.MachineName.ToString());
                Console.WriteLine(Environment.CommandLine.ToString());
                #endregion
            }
            else
            {
                services.AddDbContext<AuthenticationContext>(options => options
                    .UseMySql(Configuration.GetConnectionString("MariaDB_Windows"), mySqlOptions => mySqlOptions
                        // replace with your Server Version and Type
                        .ServerVersion(new Version(10, 4, 10), ServerType.MariaDb)));
                Console.WriteLine("operating system Windows");
                Console.WriteLine(Environment.OSVersion.ToString());
            }
            #endregion

            #region identywikacja
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationContext>();
            #endregion

            #region parametry has³a
            IServiceCollection serviceCollection = services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            });
            #endregion

            // using System.Net;
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                foreach (var ip in allowOrigins)
                    options.KnownProxies.Add(IPAddress.Parse(ip));
                //options.KnownProxies.Add(IPAddress.Parse("127.0.0.1:4200"));

            });

            #region autoryzacja za pomoc¹ JSON Web Tokens
            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secret"].ToString());
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.Use(async (ctx, next) =>
            {
                #region pomocnicze logowanie
                Console.WriteLine("Jarek test");
                Console.WriteLine($"Request Method: {ctx.Request.Method}{Environment.NewLine}");
                Console.WriteLine($"Request Scheme: {ctx.Request.Scheme}{Environment.NewLine}");
                Console.WriteLine($"Request Path: {ctx.Request.Path}{Environment.NewLine}");
                Console.WriteLine($"Request Headers:{Environment.NewLine}");

                // Headers
                foreach (var header in ctx.Request.Headers)
                    Console.WriteLine($"{header.Key}: " + $"{header.Value}{Environment.NewLine}");

                // Connection: RemoteIp
                Console.WriteLine($"Request RemoteIp: {ctx.Connection.RemoteIpAddress}");
                #endregion

                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
