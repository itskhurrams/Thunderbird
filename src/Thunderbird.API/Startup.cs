using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using Thunderbird.API.Middleware;
using Thunderbird.Domain.Models;
using Thunderbird.Infrastructure.IOC.Container;

namespace Thunderbird.API {
    public class Startup {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddSingleton(_configuration);
            services.AddMemoryCache();
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddHealthChecks()
                .AddCheck<SqlServerHealthCheck>("sql-server");
            AddCors(services);
            AddRateLimiting(services);
            AddAuthentication(services);
            RegisterServices(services);
        }
        private void AddCors(IServiceCollection services) {
            string[] allowedOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(options => {
                options.AddPolicy("Default", policy => {
                    if (allowedOrigins.Length > 0) {
                        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
                    }
                });
            });
        }
        private static void AddRateLimiting(IServiceCollection services) {
            services.AddRateLimiter(options => {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("auth", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            });
        }
        private void AddAuthentication(IServiceCollection services) {
            services.AddOptions<TokenAuthenticationSettings>()
                .Bind(_configuration.GetSection("TokenAuthentication"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var tokenSettings = _configuration.GetSection("TokenAuthentication").Get<TokenAuthenticationSettings>()
                ?? throw new InvalidOperationException("TokenAuthentication section is not configured.");
            if (string.IsNullOrEmpty(tokenSettings.SecretKey)) {
                throw new InvalidOperationException("TokenAuthentication:SecretKey is not configured.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = tokenSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = tokenSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
                        ValidateLifetime = true
                    };
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            if (context.Request.Cookies.TryGetValue(tokenSettings.CookieName, out var token)) {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
        private static void RegisterServices(IServiceCollection services) {
            DependencyContainer.RegisterServices(services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseExceptionHandler();

            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("Default");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
