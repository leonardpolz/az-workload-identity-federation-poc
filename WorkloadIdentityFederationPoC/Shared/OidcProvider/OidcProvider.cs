using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace WorkloadIdentityFederationPoC.Shared.JwtProvider;

public class OidcProvider(IOptions<OidcOptions> jwtOptions) : IOidcProvider
{
    private readonly RSA _privateKey = RSA.Create(2048);
    
    public object GetJwkPublicKey()
    {
        var parameters = _privateKey.ExportParameters(false);

        return new
        {
            kty = "RSA",
            use = "sig",
            kid = GenerateKeyId(), 
            n = Base64UrlEncoder.Encode(parameters.Modulus),
            e = Base64UrlEncoder.Encode(parameters.Exponent)
        }; 
    }    
    
    private string GenerateKeyId()
    {
        var parameters = _privateKey.ExportParameters(false);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(parameters.Modulus);
        return Base64UrlEncoder.Encode(hash);
    }

    public string GenerateJwt()
    {
        var options = jwtOptions.Value;

        var rsaKey = new RsaSecurityKey(_privateKey)
        {
            KeyId = GenerateKeyId() 
        };

        var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256); 

        var claims = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, options.SubjectIdentifier),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        });

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims.Claims,
            null,
            DateTime.UtcNow.AddHours(options.TokenLifeTimeInHours),
            signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }
}