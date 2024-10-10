namespace WorkloadIdentityFederationPoC.Shared.JwtProvider;

public interface IOidcProvider
{
    string GenerateJwt();
    object GetJwkPublicKey();
}