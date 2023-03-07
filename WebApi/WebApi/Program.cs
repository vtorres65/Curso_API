using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CursosDB>(opt => opt.UseInMemoryDatabase("DataBase"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/cursos", async (CursosDB db) => await db.Cursos.ToListAsync());

app.MapPost("/cursos", async (Curso curso, CursosDB db) =>
{
    db.Cursos.Add(curso);
    await db.SaveChangesAsync();  //INSERT INTO
    return Results.Created($"/cursos/{curso.Id}", curso);
});

app.MapPut("/cursos/{id}", async (int id, Curso inputCurso, CursosDB db) =>
{
    var curso = await db.Cursos.FindAsync(id);
    if (curso is null)
    {
        return Results.NotFound();
    }
    curso.Name = inputCurso.Name;
    curso.Duration = inputCurso.Duration;

    await db.SaveChangesAsync();  //UPDATE

    return Results.NoContent();
});

app.Run();

class Curso
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }
}

class CursosDB : DbContext
{
    public CursosDB(DbContextOptions<CursosDB> options) : base(options)
    {

    }

    public DbSet<Curso> Cursos => Set<Curso>();
}