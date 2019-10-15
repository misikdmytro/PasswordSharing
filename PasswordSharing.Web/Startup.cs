using System;
using System.IO;
using System.Linq;
using Consul;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordSharing.Algorithms;
using PasswordSharing.Interfaces;
using PasswordSharing.Redis;
using PasswordSharing.Services;
using PasswordSharing.Web.Configs;
using PasswordSharing.Web.HealthChecks;
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
        private const string DefaultAddress = "http://localhost:5000";

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                var filePath = Path.Combine(AppContext.BaseDirectory, "PasswordSharing.Web.xml");
                c.IncludeXmlComments(filePath);
            });

            var appConfig = Configuration.GetSection("service").Get<ApplicationConfig>();

            services.AddHealthChecks()
                .AddRedis(appConfig.ConnectionStrings.Redis.ConnectionString, "redis")
                .AddCheck<BaseHealthCheck>("base");

            RegisterDependencies(services);

            _logger.Information("Finished services configuration.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
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

            app.UseHealthChecks("/healthcheck");

            RegisterWithConsul(app, lifetime);

            _logger.Information("Finished application configuring.");
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

            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = appConfig.Consul.Url;
                consulConfig.Address = new Uri(address);
            }));

            _logger.Information("Finished registering dependencies.");
        }

        private void RegisterWithConsul(IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            try
            {
                _logger.Information("Starting registration in consul...");

                var consulClient = app.ApplicationServices
                    .GetRequiredService<IConsulClient>();

                var appConfig = Configuration.GetSection("service").Get<ApplicationConfig>();

                var features = app.Properties["server.Features"] as FeatureCollection;
                var addresses = features?.Get<IServerAddressesFeature>();
                var address = addresses?.Addresses.FirstOrDefault() ?? DefaultAddress;

                // Register service with consul
                _logger.Information("Register service {serviceName} located on {address}", appConfig.Consul.ServiceName, address);

                var uri = new Uri(address);

                var registration = new AgentServiceRegistration
                {
                    ID = $"{appConfig.Consul.ServiceId}-{uri.Port}",
                    Name = appConfig.Consul.ServiceName,
                    Address = $"{uri.Scheme}://{uri.Host}",
                    Port = uri.Port,
                    Tags = new[] { "microservice", "password", "sharing" },
                    Checks = new AgentServiceCheck[]
                    {
                        new AgentCheckRegistration
                        {
                            HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/healthcheck",
                            Interval = TimeSpan.FromMinutes(1),
                            Timeout = TimeSpan.FromSeconds(3),
                            Notes = "Check /healthcheck on service"
                        }
                    }
                };

                consulClient.Agent.ServiceDeregister(registration.ID).GetAwaiter().GetResult();
                consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                lifetime.ApplicationStopping.Register(() =>
                {
                    _logger.Information("Deregister from consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).GetAwaiter().GetResult();
                });

                _logger.Information("Finished registration in consul...");
            }
            catch (Exception e)
            {
                _logger.Warning(e, "Failed to register app in consul.");
            }
        }
    }
}
