using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace YZ.EFCore {

    public static class DbContextHelpers {
        public static ModelBuilder<T> ForSet<T>(this ModelBuilder modelBuilder) where T : class => new ModelBuilder<T>(modelBuilder);
        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T entity, bool isNew) where T : class {
            if (entity == null) throw new ArgumentNullException("entity");
            if (isNew) dbSet.Add(entity); else dbSet.Update(entity);
        }
    }

    public abstract class DbContextBase<Db> : DbContext where Db : DbContextBase<Db> {

        public DbContextBase(DbContextOptions<Db> options) : base(options) { }

    }
}
