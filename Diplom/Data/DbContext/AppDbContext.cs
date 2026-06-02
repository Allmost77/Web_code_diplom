using Diplom.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Data.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<LightingObject> LightingObjects => Set<LightingObject>();
    public DbSet<LightingObjectStatus> LightingObjectStatuses => Set<LightingObjectStatus>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<WorkTaskStatus> TaskStatuses => Set<WorkTaskStatus>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureLightingObjectStatuses(modelBuilder);
        ConfigureLightingObjects(modelBuilder);
        ConfigureTaskStatuses(modelBuilder);
        ConfigureTasks(modelBuilder);
        ConfigureRoles(modelBuilder);
        ConfigureUsers(modelBuilder);
    }

    private static void ConfigureLightingObjectStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LightingObjectStatus>(entity =>
        {
            entity.ToTable("lightingobjectstatuses");

            entity.HasKey(status => status.Id);
            entity.Property(status => status.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(status => status.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(status => status.Name).IsUnique();
        });
    }

    private static void ConfigureLightingObjects(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LightingObject>(entity =>
        {
            entity.ToTable("lightingobjects");

            entity.HasKey(lightingObject => lightingObject.Id);
            entity.Property(lightingObject => lightingObject.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(lightingObject => lightingObject.InventoryNumber)
                .HasColumnName("inventorynumber")
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(lightingObject => lightingObject.Address)
                .HasColumnName("address")
                .IsRequired();
            entity.Property(lightingObject => lightingObject.Latitude)
                .HasColumnName("latitude")
                .HasPrecision(10, 7);
            entity.Property(lightingObject => lightingObject.Longitude)
                .HasColumnName("longitude")
                .HasPrecision(10, 7);
            entity.Property(lightingObject => lightingObject.StatusId).HasColumnName("statusid");
            entity.Property(lightingObject => lightingObject.Description).HasColumnName("description");

            entity.HasIndex(lightingObject => lightingObject.InventoryNumber).IsUnique();
            entity.HasOne(lightingObject => lightingObject.Status)
                .WithMany(status => status.LightingObjects)
                .HasForeignKey(lightingObject => lightingObject.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTaskStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkTaskStatus>(entity =>
        {
            entity.ToTable("taskstatuses");

            entity.HasKey(status => status.Id);
            entity.Property(status => status.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(status => status.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(status => status.Name).IsUnique();
        });
    }

    private static void ConfigureTasks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);
            entity.Property(task => task.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(task => task.LightingObjectId).HasColumnName("lightingobjectid");
            entity.Property(task => task.CreatedByUserId).HasColumnName("createdbyuserid");
            entity.Property(task => task.AssignedToUserId).HasColumnName("assignedtouserid");
            entity.Property(task => task.StatusId).HasColumnName("statusid");
            entity.Property(task => task.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(task => task.Description).HasColumnName("description");
            entity.Property(task => task.CreatedAt)
                .HasColumnName("createdat")
                .HasColumnType("timestamp without time zone");
            entity.Property(task => task.Deadline)
                .HasColumnName("deadline")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(task => task.LightingObject)
                .WithMany(lightingObject => lightingObject.Tasks)
                .HasForeignKey(task => task.LightingObjectId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(task => task.CreatedByUser)
                .WithMany(user => user.CreatedTasks)
                .HasForeignKey(task => task.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(task => task.AssignedToUser)
                .WithMany(user => user.AssignedTasks)
                .HasForeignKey(task => task.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(task => task.Status)
                .WithMany(status => status.Tasks)
                .HasForeignKey(task => task.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(user => user.FullName)
                .HasColumnName("fullname")
                .HasMaxLength(200);
            entity.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(200);
            entity.Property(user => user.PasswordHash)
                .HasColumnName("passwordhash")
                .IsRequired();
            entity.Property(user => user.RoleId).HasColumnName("roleid");

            entity.HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(role => role.Id);
            entity.Property(role => role.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(role => role.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}
