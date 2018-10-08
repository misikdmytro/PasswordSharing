using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordSharing.Algorithms;
using PasswordSharing.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;
using PasswordSharing.Services;
using PasswordSharing.Web.Exceptions;
using PasswordSharing.Web.MediatorRequests;
using PasswordSharing.Web.Middlewares;

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

			services.AddDbContext<Contexts.ApplicationContext>(optionsBuilder =>
			{
				optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			var expirationSection = Configuration.GetSection("ExpirationTime");
			var expiration = expirationSection.Get<ExpirationTime>();

			var builder = new ContainerBuilder();

			builder.RegisterType<EncryptService>()
				.As<IEncryptService>();

			builder.RegisterType<EncryptService>()
				.As<IEncryptService>();

			builder.RegisterType<RandomStringGenerator>()
				.As<IStringGenerator>();

			builder.RegisterType<PasswordBuilder>()
				.As<IPasswordBuilder>();

			builder.RegisterType<LinkBuilder>()
				.As<ILinkBuilder>();

			builder.Register(_ => expiration);

			builder.RegisterType<DbRepository<Password>>()
				.As<IDbRepository<Password>>();

			builder.RegisterType<LinkRepository>()
				.As<ILinkRepository>();

			builder.Populate(services);

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

			app.UseMvc();
		}
	}
}
