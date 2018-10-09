using System;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentScheduler;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordSharing.Algorithms;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;
using PasswordSharing.Services;
using PasswordSharing.Web.Jobs;
using PasswordSharing.Web.Jobs.Factories;
using PasswordSharing.Web.MediatorRequests;
using PasswordSharing.Web.Middlewares;
using PasswordSharing.Web.Registries;
using Swashbuckle.AspNetCore.Swagger;
using EventHandler = PasswordSharing.Events.EventHandler;

namespace PasswordSharing.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

				var filePath = Path.Combine(AppContext.BaseDirectory, "PasswordSharing.Web.xml");
				c.IncludeXmlComments(filePath);
			});

			services.AddDbContext<ApplicationContext>(optionsBuilder =>
			{
				optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			var builder = new ContainerBuilder();

			builder.RegisterType<EncryptService>()
				.As<IEncryptService>();

			builder.RegisterType<EncryptService>()
				.As<IEncryptService>();

			builder.RegisterType<PasswordBuilder>()
				.As<IPasswordBuilder>();

			builder.RegisterType<EventTracker>()
				.As<IEventTracker>();

			builder.RegisterType<EventHandler>()
				.As<IEventHandler<PasswordCreated>>()
				.As<IEventHandler<PasswordStatusChanged>>();

			builder.RegisterType<DbRepository<Password>>()
				.As<IDbRepository<Password>>();

			builder.RegisterType<DbRepository<Event>>()
				.As<IDbRepository<Event>>();

			builder.RegisterType<DbRepository<HttpMessage>>()
				.As<IDbRepository<HttpMessage>>();

			builder.Populate(services);

			builder.RegisterType<Mediator>()
				.As<IMediator>()
				.InstancePerLifetimeScope();

			builder.RegisterType<DbCleanupJob>();

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

			return new AutofacServiceProvider(ApplicationContainer);
		}

		public IContainer ApplicationContainer { get; private set; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
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
			app.UseHttpTraficMiddleware();

			app.UseMvc();
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var context = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                context.Database.Migrate();
            }

			AppContainer.Container = ApplicationContainer;
			JobManager.JobFactory = new JobFactory();
			JobManager.UseUtcTime();
			JobManager.Initialize(new AppRegistry());
		}
	}
}
