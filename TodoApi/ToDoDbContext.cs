// using Microsoft.EntityFrameworkCore;

// namespace TodoApi;

// public partial class ToDoDbContext : DbContext
// {
//     public ToDoDbContext() { }

//     public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
//         : base(options) { }

//     public virtual DbSet<Item> Items { get; set; }
//     public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         if (!optionsBuilder.IsConfigured)
//             optionsBuilder.UseMySql("name=ToDoDB", 
//                 Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));
//     }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder
//             .UseCollation("utf8mb4_0900_ai_ci")
//             .HasCharSet("utf8mb4");

//         modelBuilder.Entity<Item>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("PRIMARY");
//             entity.ToTable("items");
//             entity.Property(e => e.Name).HasMaxLength(100);
//         });

//         modelBuilder.Entity<User>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("PRIMARY");
//             entity.ToTable("users");
//             entity.Property(e => e.Username).HasMaxLength(100);
//             entity.Property(e => e.Password).HasMaxLength(255);
//         });

//         OnModelCreatingPartial(modelBuilder);
//     }

//     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// }

using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext() { }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options) { }

    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("items"); 
            entity.Property(e => e.Id).HasColumnName("Id");
            // שיניתי כאן מ-Title ל-Name כדי שיתאים למחלקה שלך
            entity.Property(e => e.Name).HasColumnName("Name"); 
            entity.Property(e => e.IsComplete).HasColumnName("IsComplete");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Username).HasColumnName("Username");
            entity.Property(e => e.Password).HasColumnName("Password");
        });
    }
}