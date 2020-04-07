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

namespace TestAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Inject AppSettings
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;


            //Wersja dla MariaDB
            if (Environment.OSVersion.ToString().StartsWith("Unix"))
            {
                services.AddDbContext<AuthenticationContext>(options => options
                    .UseMySql(Configuration.GetConnectionString("MariaDB_Unix"), mySqlOptions => mySqlOptions
                        // replace with your Server Version and Type
                        .ServerVersion(new Version(10, 1, 44), ServerType.MariaDb)));
                Console.WriteLine("operating system Unix");
                Console.WriteLine(Environment.OSVersion.ToString());
                Console.WriteLine(Environment.UserName.ToString());
                Console.WriteLine(Environment.UserDomainName.ToString());
                Console.WriteLine(Environment.MachineName.ToString());
                Console.WriteLine(Environment.CommandLine.ToString());

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
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationContext>();

            IServiceCollection serviceCollection = services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            });
            // using System.Net;
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("192.168.0.107:4200"));
                options.KnownProxies.Add(IPAddress.Parse("192.168.0.108:4200"));
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1:4200"));
            });

            //services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            //{
            //    builder
            //        .WithOrigins("http://127.0.0.1:4200", "http://192.168.0.108:4200" , "http://127.0.0.1")
            //        .AllowAnyMethod()
            //        .AllowAnyHeader();                    
            //}));
            //Jwt Authentication
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
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                Console.Write("Jarek test");
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });
            Console.WriteLine(env.EnvironmentName.ToString());
            //przekierowanie nag��wk�w -> aby dzia�a�o CORS
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.Use(async (context, next) =>
            {
                // Request method, scheme, and path    
                Console.WriteLine($"Request Method: {context.Request.Method}{Environment.NewLine}");
                Console.WriteLine($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
                Console.WriteLine($"Request Path: {context.Request.Path}{Environment.NewLine}");
                Console.WriteLine($"Request Headers:{Environment.NewLine}");
                //logger.LogInformation($"Request Method: {context.Request.Method}{Environment.NewLine}");
                //logger.LogInformation($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
                //logger.LogInformation($"Request Path: {context.Request.Path}{Environment.NewLine}");
                //logger.LogInformation($"Request Headers:{Environment.NewLine}");
                // Headers
                foreach (var header in context.Request.Headers)
                {
                    Console.WriteLine($"{header.Key}: " + $"{header.Value}{Environment.NewLine}");
                    //logger.LogInformation($"{header.Key}: " + $"{header.Value}{Environment.NewLine}");
                }

                // Connection: RemoteIp
                Console.WriteLine($"Request RemoteIp: {context.Connection.RemoteIpAddress}");
                //logger.LogInformation($"Request RemoteIp: {context.Connection.RemoteIpAddress}");
                await next();
            });
            //app.Run(async (context) =>
            //{
            //    //context.Response.ContentType = "text/plain";
            //    await context.Response.WriteAsync("kontrola ustawie�");
            //    // Request method, scheme, and path
            //    await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
            //    Console.WriteLine($"Request Method: {context.Request.Method}{Environment.NewLine}");
            //    await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
            //    Console.WriteLine($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
            //    await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");
            //    Console.WriteLine($"Request Path: {context.Request.Path}{Environment.NewLine}");
            //    // Headers
            //    await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
            //    Console.WriteLine($"Request Headers:{Environment.NewLine}");
            //    foreach (var header in context.Request.Headers)
            //    {
            //        await context.Response.WriteAsync($"{header.Key}: " + $"{header.Value}{Environment.NewLine}");
            //        Console.WriteLine($"{header.Key}: " + $"{header.Value}{Environment.NewLine}");
            //    }
            //    await context.Response.WriteAsync(Environment.NewLine);
            //    Console.WriteLine(Environment.NewLine);
            //    // Connection: RemoteIp
            //    await context.Response.WriteAsync($"Request RemoteIp: {context.Connection.RemoteIpAddress}");
            //    Console.WriteLine($"Request RemoteIp: {context.Connection.RemoteIpAddress}");
            //});
            app.UseCors(builder =>
            {
                builder.WithOrigins(
                    Configuration["ApplicationSettings:Client_URL"].ToString(),
                    "http://192.168.0.107:4200",
                    "http://192.168.0.108:4200",
                    "http://192.168.0.120:4200",
                    "http://127.0.0.1")
                .AllowAnyHeader()
                .AllowAnyMethod();
            }
            );
            //app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
