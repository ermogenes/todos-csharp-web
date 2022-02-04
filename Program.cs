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

app.MapGet("/api/todos", ([FromServices] todosContext _db,
    [FromQuery(Name = "pending_only")] bool? pendingOnly,
    [FromQuery] string? title
) =>
{
    bool filtrarPendentes = pendingOnly ?? false;

    var query = _db.Todo.AsQueryable<Todo>();

    if (!String.IsNullOrEmpty(title))
    {
        query = query.Where(t => t.Title.Contains(title));
    }

    if (filtrarPendentes)
    {
        query = query.Where(t => !t.Done)
            .OrderByDescending(t => t.Id);
    }

    var todos = query.ToList<Todo>();

    return Results.Ok(todos);
});

app.MapGet("/api/todos/{id}", ([FromServices] todosContext _db,
    [FromRoute] int id
) =>
{
    var tarefa = _db.Todo.Find(id);

    if (tarefa == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(tarefa);
});

app.MapPost("/api/todos", ([FromServices] todosContext _db,
    [FromBody] Todo newTodo
) =>
{
    if (String.IsNullOrEmpty(newTodo.Title))
    {
        return Results.BadRequest("Não é possivel incluir tarefa sem título.");
    }

    var todo = new Todo
    {
        Title = newTodo.Title,
        Done = newTodo.Done,
    };

    _db.Add(todo);
    _db.SaveChanges();

    var todoUrl = $"/api/tarefas/{todo.Id}";

    return Results.Created(todoUrl, todo);
});

app.MapPut("/api/todos/{id}", ([FromServices] todosContext _db,
    [FromRoute] int id,
    [FromBody] Todo changedTodo
) =>
{
    if (changedTodo.Id != id)
    {
        return Results.BadRequest("Id inconsistente.");
    }

    var todo = _db.Todo.Find(id);

    if (todo == null)
    {
        return Results.NotFound();
    }

    if (String.IsNullOrEmpty(changedTodo.Title))
    {
        return Results.BadRequest("Não é permitido deixar uma tarefa sem título.");
    }

    todo.Title = changedTodo.Title;
    todo.Done = changedTodo.Done;

    _db.SaveChanges();

    return Results.Ok(todo);
});

app.MapMethods("/api/todos/{id}/done", new[] { "PATCH" }, ([FromServices] todosContext _db,
    [FromRoute] int id
) =>
{
    var todo = _db.Todo.Find(id);

    if (todo == null)
    {
        return Results.NotFound();
    }

    if (todo.Done)
    {
        return Results.BadRequest("Tarefa concluída anteriormente.");
    }

    todo.Done = true;
    _db.SaveChanges();

    return Results.Ok(todo);
});

app.MapDelete("/api/todos/{id}", ([FromServices] todosContext _db,
    [FromRoute] int id
) =>
{
    var todo = _db.Todo.Find(id);

    if (todo == null)
    {
        return Results.NotFound();
    }

    _db.Remove(todo);
    _db.SaveChanges();

    return Results.Ok();
});

app.Run();
