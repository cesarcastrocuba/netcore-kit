using System;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.SqlServer
{
  public sealed class DbContextOptionsBuilderFactory : IExtendDbContextOptionsBuilder
  {
    public DbContextOptionsBuilder Extend(
      DbContextOptionsBuilder optionsBuilder,
      IDatabaseConnectionStringFactory connectionStringFactory,
      string assemblyName)
    {
      return optionsBuilder.UseSqlServer(
          connectionStringFactory.Create(),
          sqlOptions =>
          {
            sqlOptions.MigrationsAssembly(assemblyName);
            sqlOptions.EnableRetryOnFailure(
              maxRetryCount: 15,
              maxRetryDelay: TimeSpan.FromSeconds(30),
              errorNumbersToAdd: null);
          })
        .EnableSensitiveDataLogging();
    }
  }
}
