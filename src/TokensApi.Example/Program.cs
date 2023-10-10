using Microsoft.OpenApi.Models;

using TokensApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(TokensApiConstants.JwtAuthScheme)
    .AddTokensApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options => options.AddBackendPolicy());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddAuthorizationHeaderInput());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// sequence matters
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
