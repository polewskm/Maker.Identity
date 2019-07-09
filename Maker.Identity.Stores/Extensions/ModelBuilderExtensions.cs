using System;
using System.Collections.Generic;
using System.Linq;
using Maker.Identity.Stores.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maker.Identity.Stores.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
        {
            return new DateTimeOffset(dateTime, offset);
        }

    }
    public static class ModelBuilderExtensions
    {
        public static PropertyBuilder<T> IsConcurrencyStamp<T>(this PropertyBuilder<T> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(50).IsRequired().IsUnicode(false).IsConcurrencyToken();
        }

        public static PropertyBuilder<T> UseIdGen<T>(this PropertyBuilder<T> propertyBuilder, bool useIdGen = true)
        {
            return propertyBuilder.ValueGeneratedNever().HasAnnotation("UseIdGen", useIdGen);
        }

        public static void UseIdGen(this ModelBuilder modelBuilder, IdValueGenerator idValueGenerator)
        {
            var properties = modelBuilder.Model.GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties()
                    .Where(prop => prop.GetAnnotations()
                        .Any(annotation => annotation.Name == "UseIdGen" && (bool?)annotation.Value == true)));

            foreach (var property in properties)
            {
                property.SetValueGeneratorFactory((prop, type) => idValueGenerator);
            }
        }

        public static EntityTypeBuilder<TEntity> AsTag<TEntity>(this EntityTypeBuilder<TEntity> entityBuilder, string name, string schema)
            where TEntity : Tag, ISupportConcurrencyToken
        {
            entityBuilder.ToTable(name, schema);

            entityBuilder.Property(_ => _.NormalizedKey).HasMaxLength(256).IsRequired().IsUnicode(false);
            entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

            entityBuilder.Property(_ => _.Key).HasMaxLength(256).IsRequired().IsUnicode(false);
            entityBuilder.Property(_ => _.Value).HasMaxLength(256).IsRequired().IsUnicode(false);

            return entityBuilder;
        }

        public static void EntityWithHistory<TEntity, TBase, THistory>(this ModelBuilder modelBuilder, string historyTableName, Action<EntityTypeBuilder<TEntity>> entityBuildAction, Action<EntityTypeBuilder<THistory>> historyBuildAction = null)
            where TEntity : class, TBase
            where TBase : ISupportAssign<TBase>
            where THistory : class, TBase, IHistoryEntity<TBase>
        {
            modelBuilder.Entity<TEntity>(entityBuilder =>
            {
                entityBuildAction(entityBuilder);

                var entityMetadata = entityBuilder.Metadata;
                var entitySchema = entityMetadata.Relational().Schema;
                var entityProperties = entityMetadata.GetProperties();

                modelBuilder.Entity<THistory>(historyBuilder =>
                {
                    var historyMetadata = historyBuilder.Metadata;
                    var historyKeyProperties = new List<IMutableProperty>();

                    historyBuilder.ToTable(historyTableName, entitySchema);

                    var propTransactionId = historyBuilder.Property(_ => _.TransactionId).UseIdGen();

                    foreach (var entityProperty in entityProperties)
                    {
                        // the history table doesn't need to keep track of concurrency tokens
                        if (entityProperty.IsConcurrencyToken) continue;

                        var historyProperty = historyMetadata.FindProperty(entityProperty.Name);
                        if (historyProperty == null)
                            throw new InvalidOperationException("TODO");

                        historyProperty.IsNullable = entityProperty.IsNullable;
                        historyProperty.IsUnicode(entityProperty.IsUnicode());
                        historyProperty.SetMaxLength(entityProperty.GetMaxLength());
                        historyProperty.SetValueComparer(entityProperty.GetValueComparer());
                        historyProperty.SetValueConverter(entityProperty.GetValueConverter());
                    }

                    var entityKey = entityMetadata.FindPrimaryKey();
                    if (entityKey == null)
                        throw new InvalidOperationException("TODO");

                    // we must add the keys for history in the same order as the keys from the entity
                    foreach (var entityProperty in entityKey.Properties)
                    {
                        var historyProperty = historyMetadata.FindProperty(entityProperty.Name);
                        if (historyProperty == null)
                            throw new InvalidOperationException("TODO");

                        historyKeyProperties.Add(historyProperty);
                    }

                    historyBuilder.Property(_ => _.CreatedWhenUtc).IsRequired();
                    var propRetiredWhen = historyBuilder.Property(_ => _.RetiredWhenUtc).IsRequired();

                    historyKeyProperties.Add(propRetiredWhen.Metadata);
                    historyKeyProperties.Add(propTransactionId.Metadata);

                    historyMetadata.SetPrimaryKey(historyKeyProperties);

                    historyBuildAction?.Invoke(historyBuilder);
                });
            });
        }

    }
}