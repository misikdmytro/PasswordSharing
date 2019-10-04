namespace PasswordSharing.Interfaces
{
	public interface IRandomBase64StringGenerator
	{
		string Generate(int originalLength);
	}
}
