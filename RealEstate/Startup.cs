﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealEstate.Models;
using RealEstate.Models.Auth;
using RealEstate.Servicios;
using RealEstate.Utilities;
using RealEstate.Utilities.Auth;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;
using System.Collections.ObjectModel;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

namespace RealEstate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => 
            {
                options.Filters.Add(typeof(ExceptionCatcherFiltrer));
            }).AddNewtonsoftJson().
                AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 28));

            services.AddDbContext<RealEstateProjectContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MyDB"), serverVersion);
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });



            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<NewIdentityUser, IdentityRole>().
                AddEntityFrameworkStores<RealEstateProjectContext>().
                AddDefaultTokenProviders();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredUniqueChars = 3;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters + "*´!# /$";
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });

            services.AddAuthorization();

            services.AddCors(options => 
            {
                options.AddPolicy("Generic", policy => 
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            services.AddScoped<IAuthSign_Up,TokenAuthSignUp>();
            services.AddScoped<IAuthLog_In, TokenAuthLogIn>();
            services.AddTransient<IGetUserInfo,GetUserInfo>();
            services.AddScoped<IEmailSender, MailJetEmailSender>();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });

            var logConnectionString = Configuration.GetConnectionString("LogErrors");
            var columnOptions = new ColumnOptions()
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn("TypeException",SqlDbType.VarChar),
                    new SqlColumn("Source",SqlDbType.VarChar),
                    new SqlColumn("StackTrace",SqlDbType.VarChar),
                    new SqlColumn("HelpLink",SqlDbType.VarChar),
                    new SqlColumn("Place",SqlDbType.VarChar),
                    new SqlColumn("TypeMember",SqlDbType.VarChar),
                    new SqlColumn("IP",SqlDbType.VarChar),
                    new SqlColumn("QueryString",SqlDbType.VarChar),
                    new SqlColumn("RouteValues",SqlDbType.VarChar)
                }
            };

            Log.Logger = new LoggerConfiguration().
                Enrich.FromLogContext().
                WriteTo.MSSqlServer(logConnectionString, sinkOptions: new SinkOptions
                {
                    TableName = "LogErrors",
                    AutoCreateSqlTable = true
                }, null, null, LogEventLevel.Error, null, columnOptions: columnOptions, null, null).
                MinimumLevel.Override("Microsoft", LogEventLevel.Error).
                CreateLogger();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("Generic");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
