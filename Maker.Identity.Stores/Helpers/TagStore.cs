using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Stores.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores.Helpers
{
    public class TagStore<TContext, TEntity, TBase, THistory> : StoreBase<TContext, TEntity, TBase, THistory>
        where TContext : DbContext
        where TBase : Tag<TBase>, ISupportAssign<TBase>
        where TEntity : class, TBase, ISupportConcurrencyToken, new()
        where THistory : class, TBase, IHistoryEntity<TBase>, new()
    {
        private readonly Func<TEntity> _factory;

        public TagStore(TContext context, Func<TEntity, Expression<Func<THistory, bool>>> retirePredicateFactory, Func<TEntity> factory, IdentityErrorDescriber describer = null)
            : base(context, retirePredicateFactory, describer)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            AutoSaveChanges = false;
        }

        public virtual async Task UpdateTagsAsync(IEnumerable<KeyValuePair<string, string>> tags, IReadOnlyDictionary<string, TEntity> existingTags, CancellationToken cancellationToken)
        {
            var dirty = false;
            var visitor = new HashSet<string>(StringComparer.Ordinal);

            foreach (var tag in tags)
            {
                var normalizedKey = tag.Key.Normalize().ToUpperInvariant();
                visitor.Add(normalizedKey);

                if (existingTags.TryGetValue(normalizedKey, out var existingTag))
                {
                    if (!string.Equals(existingTag.Key, tag.Key, StringComparison.Ordinal))
                    {
                        existingTag.Key = tag.Key;
                        dirty = true;
                    }

                    if (!string.Equals(existingTag.Value, tag.Value, StringComparison.Ordinal))
                    {
                        existingTag.Value = tag.Value;
                        dirty = true;
                    }

                    if (!dirty) continue;

                    await UpdateAsync(existingTag, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var newTag = _factory();
                    newTag.NormalizedKey = normalizedKey;
                    newTag.Key = tag.Key;
                    newTag.Value = tag.Value;

                    await CreateAsync(newTag, cancellationToken).ConfigureAwait(false);
                    dirty = true;
                }
            }

            var tagsToBeRemoved = existingTags.Values.Where(existingTag => !visitor.Contains(existingTag.Key)).ToList();
            if (tagsToBeRemoved.Count > 0)
            {
                await DeleteAsync(tagsToBeRemoved, cancellationToken).ConfigureAwait(false);
                dirty = true;
            }

            if (!dirty) return;

            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

    }
}