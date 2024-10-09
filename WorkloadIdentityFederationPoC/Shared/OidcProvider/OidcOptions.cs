namespace WorkloadIdentityFederationPoC.Shared.JwtProvider;

public class OidcOptions
{
    public required string Base { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int TokenLifeTimeInHours { get; init; }

    public required string SubjectIdentifier { get; init; }
}