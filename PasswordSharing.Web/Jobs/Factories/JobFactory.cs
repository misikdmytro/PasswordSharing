using Autofac;
using FluentScheduler;

namespace PasswordSharing.Web.Jobs.Factories
{
	public class JobFactory : IJobFactory
	{
		public IJob GetJobInstance<T>() where T : IJob
		{
			return AppContainer.GetJobInstance<T>();
		}
	}

	public class AppContainer
	{
		public static IContainer Container { get; set; }

		public static IJob GetJobInstance<T>() where T : IJob
		{
			return Container.Resolve<T>();
		}
	}
}
