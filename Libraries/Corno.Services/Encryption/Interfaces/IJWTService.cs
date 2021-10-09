namespace Corno.Services.Encryption.Interfaces
{
    public interface IJWTService
    {
        string GenerateToken(string payload);
        string DecryptToken(string response);
    }
}
