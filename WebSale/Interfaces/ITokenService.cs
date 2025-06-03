using System.Security.Claims;
using WebSale.Dto.Auth;

namespace WebSale.Interfaces
{
    public interface ITokenService
    {
        TokenDto GetToken(IEnumerable<Claim> claim);
        TokenDto GetTokenResetPassword(IEnumerable<Claim> claim);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
