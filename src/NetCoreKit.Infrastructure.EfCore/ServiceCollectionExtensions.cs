using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.Repository;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Infrastructure.EfCore
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddEfCore(this IServiceCollection services)
    {
      var serviceProvider = services.BuildServiceProvider();
      var fullyQualifiedPrefix = serviceProvider
        .GetRequiredService<IConfiguration>()
        .GetValue<string>("EfCore:FullyQualifiedPrefix");

      var entityTypes = fullyQualifiedPrefix
        .LoadAssemblyWithPattern()
        .SelectMany(m => m.DefinedTypes)
        .Where(x => typeof(IEntity).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract);

      foreach (var entity in entityTypes)
      {
        var repoType = typeof(IEfRepositoryAsync<>).MakeGenericType(entity);
        var implRepoType = typeof(EfRepositoryAsync<>).MakeGenericType(entity);
        services.AddScoped(repoType, implRepoType);

        var queryRepoType = typeof(IEfQueryRepository<>).MakeGenericType(entity);
        var implQueryRepoType = typeof(EfQueryRepository<>).MakeGenericType(entity);
        services.AddScoped(queryRepoType, implQueryRepoType);
      }

      services.AddScoped(
        typeof(IUnitOfWorkAsync), resolver =>
          new EfUnitOfWork(
            resolver.GetService<DbContext>(),
            resolver.GetService<IServiceProvider>()));

      services.AddScoped(
        typeof(IQueryRepositoryFactory), resolver =>
          new EfQueryRepositoryFactory(resolver.GetService<IServiceProvider>()));

      // by default, we register the in-memory database
      services.AddScoped(typeof(IDatabaseConnectionStringFactory), typeof(NoOpDatabaseConnectionStringFactory));
      services.AddScoped(typeof(IExtendDbContextOptionsBuilder), typeof(InMemoryDbContextOptionsBuilderFactory));

      return services;
    }
  }

  public class NoOpDatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
  {
    public string Create()
    {
      return string.Empty;
    }
  }

  public class InMemoryDbContextOptionsBuilderFactory : IExtendDbContextOptionsBuilder
  {
    public DbContextOptionsBuilder Extend(
        DbContextOptionsBuilder optionsBuilder,
        IDatabaseConnectionStringFactory connectionStringFactory,
        string assemblyName)
    {
      return optionsBuilder.UseSqlite(
          "Data Source=App_Data\\localdb.db",
          sqlOptions =>
          {
            sqlOptions.MigrationsAssembly(assemblyName);
          });
    }
  }
}
