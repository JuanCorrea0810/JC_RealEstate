using RealEstate.Models.Auth;

namespace RealEstate.Utilities.Auth
{
    public interface IAuthLog_In
    {
        public Task<ResponseAuth> Token(LoginAuth credentials);
    }
}
