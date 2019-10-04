namespace PasswordSharing.Interfaces
{
	public interface IRandomStringGenerator
	{
		string Generate(int originalLength);
	}
}
