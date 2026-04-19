using TodoApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("ToDoDB") ?? builder.Configuration["ConnectionStrings:ToDoDB"];
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// CORS - פתוח לכולם כדי למנוע חסימות
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKey1234567890123456";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

// Routes
app.MapPost("/register", async (ToDoDbContext db, User user) => {
    if (await db.Users.AnyAsync(u => u.Username == user.Username)) return Results.BadRequest();
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/login", async (ToDoDbContext db, User user) => {
    var u = await db.Users.FirstOrDefaultAsync(x => x.Username == user.Username && x.Password == user.Password);
    if (u == null) return Results.Unauthorized();
    
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim("id", u.Id.ToString()) }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    return Results.Ok(new { token = tokenHandler.CreateToken(tokenDescriptor) == null ? "" : tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)) });
});

// Items Routes
app.MapGet("/items", async (ToDoDbContext db) => await db.Items.ToListAsync()).RequireAuthorization();
app.MapPost("/items", async (ToDoDbContext db, Item item) => {
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
}).RequireAuthorization();

// הקוד של יצירת הטבלאות - חייב להיות לפני app.Run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    try 
    {
        // שלב 1: מחיקת הטבלה הקיימת כדי לנקות שגיאות מבנה קודמות
        db.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS items;");
        
        // שלב 2: יצירה מחדש של הטבלה עם השדות המדויקים (Name ולא Title)
        var createItemsSql = @"
            CREATE TABLE items (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(255) NOT NULL,
                IsComplete BOOLEAN DEFAULT FALSE
            );";

        // יצירת טבלת משתמשים (אם היא לא קיימת)
        var createUsersSql = @"
            CREATE TABLE IF NOT EXISTS users (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Username VARCHAR(255) NOT NULL,
                Password VARCHAR(255) NOT NULL
            );";

        db.Database.ExecuteSqlRaw(createUsersSql);
        db.Database.ExecuteSqlRaw(createItemsSql);
        
        Console.WriteLine("---- Database Cleaned and Resetted Successfully ----");
    }
    catch (Exception ex) 
    {
        Console.WriteLine($"---- DATABASE ERROR: {ex.Message} ----");
    }
}

app.Run();