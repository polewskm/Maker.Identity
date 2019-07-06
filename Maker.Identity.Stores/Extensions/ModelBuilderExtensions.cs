using System;
using System.Collections.Generic;
using Maker.Identity.Stores.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maker.Identity.Stores.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static EntityTypeBuilder<TEntity> AsTag<TEntity>(this EntityTypeBuilder<TEntity> builder, string name, string schema)
            where TEntity : Tag, ISupportConcurrencyToken
        {
            builder.ToTable(name, schema);

            builder.Property(_ => _.NormalizedKey).HasMaxLength(256).IsRequired().IsUnicode(false);
            builder.Property(_ => _.ConcurrencyStamp).HasMaxLength(50).IsRequired().IsUnicode(false).IsConcurrencyToken();

            builder.Property(_ => _.Key).HasMaxLength(256).IsRequired().IsUnicode(false);
            builder.Property(_ => _.Value).HasMaxLength(256).IsRequired().IsUnicode(false);

            return builder;
        }

        public static void EntityWithHistory<TEntity, TBase, THistory>(this ModelBuilder modelBuilder, string historyTableName, string historySchemaName, Action<EntityTypeBuilder<TEntity>> entityBuildAction, Action<EntityTypeBuilder<THistory>> historyBuildAction = null)
            where TEntity : class, TBase
            where TBase : ISupportAssign<TBase>
            where THistory : class, TBase, IHistoryEntity<TBase>
        {
            modelBuilder.Entity<TEntity>(entityBuilder =>
            {
                entityBuildAction(entityBuilder);

                var entityMetadata = entityBuilder.Metadata;
                var entityProperties = entityMetadata.GetProperties();

                modelBuilder.Entity<THistory>(historyBuilder =>
                {
                    var historyMetadata = historyBuilder.Metadata;
                    var historyKeyProperties = new List<IMutableProperty>();

                    historyBuilder.ToTable(historyTableName, historySchemaName);

                    var propTransactionId = historyBuilder.Property(_ => _.TransactionId).HasMaxLength(50).IsRequired().IsUnicode(false);

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

                    historyBuilder.Property(_ => _.CreatedWhen).IsRequired();
                    var propRetiredWhen = historyBuilder.Property(_ => _.RetiredWhen).IsRequired();

                    historyKeyProperties.Add(propRetiredWhen.Metadata);
                    historyKeyProperties.Add(propTransactionId.Metadata);

                    historyMetadata.SetPrimaryKey(historyKeyProperties);

                    historyBuildAction?.Invoke(historyBuilder);
                });
            });
        }

    }
}