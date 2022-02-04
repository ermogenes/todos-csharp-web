# todos-csharp-web
Uma aplicação TODO em C# (web) com Minimal APIs, EFCore 6, MySQL e Pomelo

## OpenAPI (Swagger)

Pacote:
```
dotnet add package Swashbuckle.AspNetCore
```

Código:
```cs
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
...
app.UseSwagger();
app.UseSwaggerUI();
```

## _Static Files_

Código:
```cs
app.UseDefaultFiles();
app.UseStaticFiles();
```

## EntityFramework Core 6 (MySQL com Pomelo)

Banco: https://github.com/ermogenes/todos-mysql

Subir o banco rapidamente com Docker (MySQL 8.0.23):

```
docker run -p 3333:3306 -e MYSQL_ROOT_PASSWORD=1234 ermogenes/top5-mysql
```

Ou então assim, e faça a carga da [estrutura e dados](https://github.com/ermogenes/top5-mysql/blob/master/scripts/top5.sql) manualmente  (MySQL 8.0.28):

```
docker run -p 3333:3306 -e MYSQL_ROOT_PASSWORD=1234 mysql:8.0.28
```

Foi utilizada a lib [Pomelo](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) `Pomelo.EntityFrameworkCore.MySql` em vez do Connector/NET oficial da Oracle, devido ao suporte simplificado a diferentes versões do MySQL.

Comandos utilizados para fazer o _scaffolding_:

```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Pomelo.EntityFrameworkCore.MySql

dotnet ef dbcontext scaffold "server=localhost;port=3333;uid=root;pwd=1234;database=todos" Pomelo.EntityFrameworkCore.MySql -o db -f --no-pluralize
```

String de conexão transferida de `top5Context.OnConfiguring` para `appsettings.json`:

```json
"ConnectionStrings": {
    "top5Connection": "server=localhost;port=3333;uid=root;pwd=1234;database=todos"
}
```

Código:
```cs
...
builder.Services.AddDbContext<todosContext>(opt =>
{
    string connectionString = builder.Configuration.GetConnectionString("todosConnection");
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    opt.UseMySql(connectionString, serverVersion);
});
...
app.MapGet("/api/todos", ([FromServices] todosContext _db) => {
    return _db.Todo.ToList();
});
...
```



