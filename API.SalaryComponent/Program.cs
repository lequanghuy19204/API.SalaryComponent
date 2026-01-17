using Core.SalaryComponent.Interfaces.IRepository;
using Core.SalaryComponent.Interfaces.IServices;
using Core.SalaryComponent.Services;
using Infrastructure.SalaryComponent.Repositories;
using API.SalaryComponent.Middlewares;
using Dapper;

DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISalaryCompositionRepository, SalaryCompositionRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<ISalaryCompositionSystemRepository, SalaryCompositionSystemRepository>();
builder.Services.AddScoped<IGridConfigRepository, GridConfigRepository>();
builder.Services.AddScoped<ISalaryCompositionService, SalaryCompositionService>();
builder.Services.AddScoped<ISalaryCompositionSystemService, SalaryCompositionSystemService>();
builder.Services.AddScoped<IGridConfigService, GridConfigService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
