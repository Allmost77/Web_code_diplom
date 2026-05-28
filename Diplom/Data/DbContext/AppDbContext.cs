using Microsoft.EntityFrameworkCore;

namespace Diplom.Data.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
   public DbSet<Models.UnoModel> UnoModel => Set<Models.UnoModel>();

   public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
   {
      
   }
}