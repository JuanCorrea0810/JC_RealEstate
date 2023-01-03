using RealEstate.Models.Auth;

namespace RealEstate.Utilities.Auth
{
    public interface IAuthSign_Up
    {
        public Task<ResponseAuth> Token(InfoUser credentials);
    }
}
