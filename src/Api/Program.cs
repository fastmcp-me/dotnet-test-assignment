using Api.DependencyInjections;
using Api.Middlewares;
using Application.DependencyInjections;
using Infrastructure.DependencyInjections;

var builder = WebApplication.CreateBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add services to the container.
builder.Services
    .AddApiServices(builder.Configuration, builder.Environment)
    .AddInfrastructureServices(builder.Configuration, builder.Environment)
    .AddDalServices(builder.Configuration, builder.Environment)
    .AddApplicationServices(builder.Configuration, builder.Environment)
    .AddAutoMapServices(builder.Configuration, builder.Environment);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CurrentUserMiddleware>();


app.MapControllers();

app.Run();