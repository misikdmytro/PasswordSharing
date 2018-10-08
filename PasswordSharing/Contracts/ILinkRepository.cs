using System.Threading.Tasks;
using PasswordSharing.Models;

namespace PasswordSharing.Contracts
{
	public interface ILinkRepository : IDbRepository<Link>
	{
		Task<Link> GetByPasswordId(int passwordId);
		Task DeleteByPasswordId(int passwordId);
	}
}