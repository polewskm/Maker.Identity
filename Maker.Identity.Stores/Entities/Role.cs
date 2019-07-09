using System;

namespace Maker.Identity.Stores.Entities
{
    public interface IRoleBase
    {
        long RoleId { get; set; }

        string NormalizedName { get; set; }

        string Name { get; set; }
    }

    public abstract class RoleBase : RoleBase<RoleBase>
    {
        // nothing
    }

    public abstract class RoleBase<TBase> : IRoleBase, ISupportAssign<TBase>
        where TBase : RoleBase<TBase>
    {
        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        public string NormalizedName { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc/>
        public virtual void Assign(TBase other)
        {
            RoleId = other.RoleId;
            NormalizedName = other.NormalizedName;
            Name = other.Name;
        }

        /// <inheritdoc/>
        public override string ToString() => Name;
    }

    public class Role : RoleBase, ISupportConcurrencyToken
    {
        /// <inheritdoc/>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class RoleHistory : RoleBase, IHistoryEntity<RoleBase>
    {
        /// <inheritdoc/>
        public long TransactionId { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedWhen { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset RetiredWhen { get; set; }
    }
}