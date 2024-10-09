namespace WorkloadIdentityFederationPoC.Shared.JwtProvider;

public interface IOidcProvider
{
    string Generate();
    object GetJwkPublicKey();
}