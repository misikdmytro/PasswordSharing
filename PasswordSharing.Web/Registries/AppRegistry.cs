using FluentScheduler;
using PasswordSharing.Web.Jobs;

namespace PasswordSharing.Web.Registries
{
	public class AppRegistry : Registry
	{
		public AppRegistry()
		{
			Schedule<DbCleanupJob>().ToRunNow().AndEvery(1).Days();
		}
	}
}
