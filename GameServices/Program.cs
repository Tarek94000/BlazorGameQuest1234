using GameService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Base InMemory (simple pour la V2)
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseInMemoryDatabase("GameQuestDb"));

// Swagger + MVC
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
