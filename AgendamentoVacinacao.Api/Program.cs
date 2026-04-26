using AgendamentoVacinacao.WebApi.Configuration;
using AgendamentoVacinacao.WebApi.Middleware;
using AgendamentoVacinacao.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerSecurity();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDbConfiguration(builder.Configuration);
builder.Services.AddDependencyInjectionConfiguration();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontendLocal", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

await DataSeeder.SeedEnfermeiroAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("PermitirFrontendLocal");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
