namespace Thunderbird.Application.Interfaces {
    public interface IPasswordHasher {
        string Hash(string password);
        bool Verify(string hashedPassword, string providedPassword);
        bool IsHashed(string value);
    }
}
