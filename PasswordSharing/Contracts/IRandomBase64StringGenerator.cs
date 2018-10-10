namespace PasswordSharing.Contracts
{
	public interface IRandomBase64StringGenerator
	{
		string Generate(int originalLength);
	}
}
