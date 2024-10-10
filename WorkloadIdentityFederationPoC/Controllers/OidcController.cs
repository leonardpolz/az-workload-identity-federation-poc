using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WorkloadIdentityFederationPoC.Shared.JwtProvider;

// ReSharper disable InconsistentNaming

namespace WorkloadIdentityFederationPoC.Controllers;

[ApiController]
public class OidcController(IOptions<OidcOptions> jwtOptions, IOidcProvider oidcProvider) : ControllerBase
{
    [HttpGet]
    [Route(".well-known/openid-configuration")]
    public async Task<IActionResult> GetOpenIdConfigurationAsync()
    {
        var options = jwtOptions.Value;

        var openIdConfig = new
        {
            issuer = options.Issuer,
            authorization_endpoint = $"{options.Base}/authorize",
            token_endpoint = $"{options.Base}/token",
            jwks_uri = $"{options.Base}/.well-known/jwks.json",
            response_types_supported = new[] { "code", "token", "id_token" },
            subject_types_supported = new[] { "public" },
            id_token_signing_alg_values_supported = new[] { "RS256" },
            scopes_supported = new[] { "openid", "profile", "email" },
            claims_supported = new[] { "sub", "name", "email", "preferred_username" }
        };

        return Ok(openIdConfig);
    }

    [HttpGet]
    [Route(".well-known/jwks.json")]
    public IActionResult GetJwks()
    {
        var jwk = oidcProvider.GetJwkPublicKey();

        var jwks = new
        {
            keys = new[] { jwk }
        };

        return Ok(jwks);
    }

    [Route("authorize")]
    [HttpGet]
    public IActionResult Authorize(string response_type, string client_id, string redirect_uri, string scope,
        string state)
    {
        if (response_type != "code") return BadRequest("Unsupported response type.");
        var authorizationCode = "example-code";
        var redirectUrl = $"{redirect_uri}?code={authorizationCode}&state={state}";

        return Redirect(redirectUrl);
    }

    [Route("token")]
    [HttpGet]
    public async Task<IActionResult> GetJwtAsync()
    {
        var jwtToken = oidcProvider.GenerateJwt();
        return Ok(jwtToken);
    }
}