using System;
using System.Linq;
using Maker.Identity.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maker.Identity.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        private const string KeySkipAudit = "MakerIdentity:SkipAudit";
        private const string KeyUseIdGen = "MakerIdentity:UseIdGen";

        public static bool ShouldAuditChanges(this EntityEntry entityEntry)
        {
            if (entityEntry.Entity is AuditChange || entityEntry.Metadata.SkipAudit())
                return false;

            switch (entityEntry.State)
            {
                case EntityState.Detached:
                case EntityState.Unchanged:
                    return false;
            }

            return true;
        }

        public static bool SkipAudit(this IAnnotatable item)
        {
            return item.FindAnnotation(KeySkipAudit)?.Value as bool? == true;
        }

        public static EntityTypeBuilder<T> SkipAudit<T>(this EntityTypeBuilder<T> builder, bool skipAudit = true)
            where T : class
        {
            return builder.HasAnnotation(KeySkipAudit, skipAudit);
        }

        public static PropertyBuilder<T> SkipAudit<T>(this PropertyBuilder<T> builder, bool skipAudit = true)
        {
            return builder.HasAnnotation(KeySkipAudit, skipAudit);
        }

        public static PropertyBuilder<T> IsConcurrencyStamp<T>(this PropertyBuilder<T> builder)
        {
            return builder.HasMaxLength(50).IsRequired().IsUnicode(false).IsConcurrencyToken().SkipAudit();
        }

        public static PropertyBuilder<T> UseIdGen<T>(this PropertyBuilder<T> builder, bool useIdGen = true)
        {
            return builder.ValueGeneratedNever().HasAnnotation(KeyUseIdGen, useIdGen);
        }

        public static void UseIdGen(this ModelBuilder builder, IdValueGenerator idValueGenerator)
        {
            var properties = builder.Model.GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties()
                    .Where(prop => prop.GetAnnotations()
                        .Any(annotation => annotation.Name == KeyUseIdGen && annotation.Value as bool? == true)));

            foreach (var property in properties)
            {
                property.SetValueGeneratorFactory((prop, type) => idValueGenerator);
            }
        }

        public static void UseUtcDateTime(this ModelBuilder builder)
        {
            // always persist DateTime values as UTC
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                value => value.ToUniversalTime(),
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

            var properties = builder.Model.GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties()
                    .Where(prop => typeof(DateTime?).IsAssignableFrom(prop.ClrType) && prop.GetValueConverter() == null));

            foreach (var property in properties)
            {
                property.SetValueConverter(dateTimeConverter);
            }
        }

        public static EntityTypeBuilder<TEntity> AsTag<TEntity>(this EntityTypeBuilder<TEntity> builder, string name, string schema)
            where TEntity : TagBase, ISupportConcurrencyToken
        {
            builder.ToTable(name, schema);

            builder.Property(_ => _.NormalizedKey).HasMaxLength(256).IsRequired().IsUnicode(false);
            builder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

            builder.Property(_ => _.Key).HasMaxLength(256).IsRequired().IsUnicode(false);
            builder.Property(_ => _.Value).HasMaxLength(256).IsRequired().IsUnicode(false);

            return builder;
        }

    }
}