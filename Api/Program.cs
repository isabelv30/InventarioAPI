using Api.Errors;
using Api.Global;

var builder = WebApplication.CreateBuilder(args);

// Services of the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Inyección de dependencias
InyeccionDependencias.GenerateService(builder.Configuration.GetConnectionString("DefaultConnection"));
// Manejo de errores
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
