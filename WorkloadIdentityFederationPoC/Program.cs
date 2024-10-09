using WorkloadIdentityFederationPoC.Shared.JwtProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OidcOptions>(builder.Configuration.GetSection("OidcOptions"));
builder.Services.AddSingleton<IOidcProvider, OidcProvider>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();