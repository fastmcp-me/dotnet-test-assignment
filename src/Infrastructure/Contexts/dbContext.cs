using Domain.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class dbContext : DbContext, IdbContext
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    #region DI
    public dbContext() { }

    public dbContext(DbContextOptions<dbContext> options) : base(options)
    {
        
    }
    #endregion


}