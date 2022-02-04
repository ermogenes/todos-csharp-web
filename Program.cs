using todos.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Conexão com banco
builder.Services.AddDbContext<todosContext>(opt =>
{
    string connectionString = builder.Configuration.GetConnectionString("todosConnection");
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    opt.UseMySql(connectionString, serverVersion);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicação Frontend
app.UseDefaultFiles();
app.UseStaticFiles();

// Endpoints

app.MapGet("/api/tarefas", ([FromServices] todosContext _db) => {
    var tarefas = _db.Todo
        .ToList<Todo>();

    return Results.Ok(tarefas);
});

// Continue daqui

app.Run();
