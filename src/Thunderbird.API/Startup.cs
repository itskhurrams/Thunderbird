using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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
            AddAuthentication(services);
            RegisterServices(services);
        }
        private void AddAuthentication(IServiceCollection services) {
            string secretKey = _configuration["TokenAuthentication:SecretKey"]
                ?? throw new InvalidOperationException("TokenAuthentication:SecretKey is not configured.");
            string cookieName = _configuration["TokenAuthentication:CookieName"] ?? "access_token";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["TokenAuthentication:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _configuration["TokenAuthentication:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ValidateLifetime = true
                    };
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            if (context.Request.Cookies.TryGetValue(cookieName, out var token)) {
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
            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
