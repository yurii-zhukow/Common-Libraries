using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace YZ.EFCore {

    public class ModelBuilder<T> where T : class {
        readonly ModelBuilder modelBuilder;
        internal ModelBuilder(ModelBuilder modelBuilder) {
            this.modelBuilder = modelBuilder;
        }

        ModelBuilder<T> call(Action fn) {
            fn();
            return this;
        }

        public ModelBuilder<T2> ForSet<T2>() where T2 : class => new ModelBuilder<T2>(modelBuilder);

        public ModelBuilder<T> AddKey(Expression<Func<T, object>> field) => call(() => modelBuilder.Entity<T>().HasKey(field));

        public ModelBuilder<T> Keyless() => call(() => modelBuilder.Entity<T>().HasNoKey());
        public ModelBuilder<T> AddUnique(bool clustered, params Expression<Func<T, object>>[] fields) => call(() => fields.ToList().ForEach(f => modelBuilder.Entity<T>().HasIndex(f).IsUnique().IsClustered(clustered)));
        public ModelBuilder<T> AddDefault(Expression<Func<T, object>> field, string valueSql) => call(() => modelBuilder.Entity<T>().Property(field).HasDefaultValueSql(valueSql));
        public ModelBuilder<T> AddRequired(params Expression<Func<T, object>>[] fields) => call(() => fields.ToList().ForEach(f => modelBuilder.Entity<T>().Property(f).IsRequired(true)));
        public ModelBuilder<T> JsonConvert<T2>(Expression<Func<T, T2>> field) => call(() => modelBuilder.Entity<T>().Property(field).HasConversion(t => Newtonsoft.Json.JsonConvert.SerializeObject(t), t => Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(t)));

        public ModelBuilder<T> OnDelete<T2>(Expression<Func<T, T2>> field, Expression<Func<T2, IEnumerable<T>>> foreignField, DeleteBehavior deleteBehavior) where T2 : class => call(() => modelBuilder.Entity<T>().HasOne(field).WithMany(foreignField).OnDelete(deleteBehavior));
        public ModelBuilder<T> OnDelete<T2>(Expression<Func<T, IEnumerable<T2>>> field, Expression<Func<T2, T>> foreignField, DeleteBehavior deleteBehavior) where T2 : class => call(() => modelBuilder.Entity<T>().HasMany(field).WithOne(foreignField).OnDelete(deleteBehavior));

        public ModelBuilder<T> AddIndex(params Expression<Func<T, object>>[] fields) => call(() => fields.ToList().ForEach(f => modelBuilder.Entity<T>().HasIndex(f)));
        public ModelBuilder<T> AddClustered(Expression<Func<T, object>> field) => call(() => modelBuilder.Entity<T>().HasIndex(field).IsUnique().IsClustered());

        public ModelBuilder<T> AddForeignKey<T2>(Expression<Func<T, T2>> field, Expression<Func<T2, IEnumerable<T>>> foreignField, bool required = true, DeleteBehavior deleteBehavior = DeleteBehavior.Cascade) where T2 : class => call(() => modelBuilder.Entity<T>().HasOne(field).WithMany(foreignField).IsRequired(required));
        public ModelBuilder<T> AddForeignKey<T2>(Expression<Func<T, IEnumerable<T2>>> field, Expression<Func<T2, T>> foreignField, bool required = true, DeleteBehavior deleteBehavior = DeleteBehavior.Cascade) where T2 : class => call(() => modelBuilder.Entity<T>().HasMany(field).WithOne(foreignField).IsRequired(required));
        public ModelBuilder<T> AddForeignKey<T2>(Expression<Func<T, T2>> field, Expression<Func<T2, T>> foreignField, bool required = true, DeleteBehavior deleteBehavior = DeleteBehavior.Cascade) where T2 : class => call(() => modelBuilder.Entity<T>().HasOne(field).WithOne(foreignField).IsRequired(required));
    }
}