using System;
using System.IO;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordSharing.Algorithms;
using PasswordSharing.Interfaces;
using PasswordSharing.Redis;
using PasswordSharing.Services;
using PasswordSharing.Web.Configs;
using PasswordSharing.Web.Middlewares;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Serilog.ILogger;

namespace PasswordSharing.Web
{
    public class Startup
    {
        private readonly ILogger _logger = Log.ForContext<Startup>();

        public Startup(IConfiguration configuration)
        {
            _logger.Information("Starting web host...");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.Information("Starting services configuration...");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                var filePath = Path.Combine(AppContext.BaseDirectory, "PasswordSharing.Web.xml");
                c.IncludeXmlComments(filePath);
            });

            RegisterDependencies(services);

            _logger.Information("Finished services configuration.");
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            _logger.Information("Starting registering dependencies...");

            services.AddTransient<IEncryptService, EncryptService>();
            services.AddTransient<IKeyGenerator, RsaKeyGenerator>();
            services.AddTransient<IRandomStringGenerator, RandomBase64StringGenerator>();
            services.AddTransient<IPasswordEncryptor, PasswordEncryptor>();

            var appConfig = Configuration.GetSection("service").Get<ApplicationConfig>();

            var connectionMultiplexer = ConnectionMultiplexer.Connect(appConfig.ConnectionStrings.Redis.ConnectionString);
            var redisClientFactory = new RedisClientFactory(connectionMultiplexer, appConfig.ConnectionStrings.Redis.DefaultExpiration);

            services.AddSingleton<IRedisClientFactory, RedisClientFactory>(provider => redisClientFactory);
            services.AddMediatR(typeof(Startup).Assembly);

            _logger.Information("Finished registering dependencies.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _logger.Information("Starting application configuring...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseExceptionHandlerMiddleware();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            _logger.Information("Finished application configuring.");
        }
    }
}
