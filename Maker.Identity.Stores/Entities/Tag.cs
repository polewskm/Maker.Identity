namespace Maker.Identity.Stores.Entities
{
    public abstract class Tag
    {
        public string NormalizedKey { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public abstract class Tag<TEntity> : Tag, ISupportAssign<TEntity>
        where TEntity : Tag, ISupportAssign<TEntity>
    {
        public virtual void Assign(TEntity other)
        {
            NormalizedKey = other.NormalizedKey;
            Key = other.Key;
            Value = other.Value;
        }
    }
}