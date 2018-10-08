using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordSharing.Algorithms;
using PasswordSharing.Contracts;

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

			var builder = new ContainerBuilder();

			builder.RegisterType<EncryptService>()
				.As<IEncryptService>();

			builder.Populate(services);

			ApplicationContainer = builder.Build();

			//var str = File.ReadAllText(@"privateKey.xml");
			//var privateKey = PrivateKey.FromString(str);

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

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}
