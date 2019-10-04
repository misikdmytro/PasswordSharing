using System;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
using PasswordSharing.Web.MediatorRequests;
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
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
            return new AutofacServiceProvider(ApplicationContainer);
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            _logger.Information("Starting registering dependencies...");

            var builder = new ContainerBuilder();

            builder.RegisterType<EncryptService>()
                .As<IEncryptService>();

            builder.RegisterType<RsaKeyGenerator>()
                .As<IRsaKeyGenerator>();

            builder.RegisterType<RandomBase64StringGenerator>()
                .As<IRandomBase64StringGenerator>();

            builder.RegisterType<PasswordEncryptor>()
                .As<IPasswordEncryptor>();

            builder.Populate(services);

            var appConfig = Configuration.GetSection("service").Get<ApplicationConfig>();


            var connectionMultiplexer = ConnectionMultiplexer.Connect(appConfig.ConnectionStrings.Redis.ConnectionString);
            builder.Register(c =>
                    new RedisClientFactory(connectionMultiplexer, appConfig.ConnectionStrings.Redis.DefaultExpiration))
                .As<IRedisClientFactory>();

            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency()
            builder.RegisterAssemblyTypes(typeof(GeneratePasswordLinkRequest).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            ApplicationContainer = builder.Build();

            _logger.Information("Finished registering dependencies.");
        }

        public IContainer ApplicationContainer { get; private set; }

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

            //app.UseHttpsRedirection();
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
