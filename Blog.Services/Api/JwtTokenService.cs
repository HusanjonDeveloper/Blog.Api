using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Common.Models.JwtOptions;
using Blog.Data.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services.Api;

public class JwtTokenService
{
    private readonly JwtOption _jwtOption;

    // Eski kodda bu qiymatlar shu yerda qo'lda (hardcoded) yozilgan edi va
    // appsettings.json'dagi qiymatlardan butunlay mustaqil edi - ikkitasi
    // sinxrondan chiqib qolishi mumkin edi. Endi bitta manba: appsettings.json -> JwtOption.
    public JwtTokenService(IOptions<JwtOption> jwtOption)
    {
        _jwtOption = jwtOption.Value;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role ?? "User")
        };

        var signinKey = Encoding.UTF8.GetBytes(_jwtOption.signinKey);

        var token = new JwtSecurityToken(
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOption.Minute),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(signinKey), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
