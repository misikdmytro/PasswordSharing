using System;

namespace PasswordSharing.Models
{
	public interface IIDentifiable
	{
		Guid Id { get; set; }
	}
}
