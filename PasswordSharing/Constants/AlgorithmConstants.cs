namespace PasswordSharing.Constants
{
    public static class AlgorithmConstants
    {
        public const int KeySize = 1024;
        public const int MaxMessageSize = (KeySize - 384) / 8 + 37;
	}
}
