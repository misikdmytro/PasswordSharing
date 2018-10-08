namespace PasswordSharing.Contracts
{
	public interface IEncryptService
	{
		string Decrypt(string str);
		string Encrypt(string str);
	}
}