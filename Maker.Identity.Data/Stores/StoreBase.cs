using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Stores
{
    public abstract class StoreBase : IDisposable
    {
        private bool _disposed;

        protected IdentityErrorDescriber ErrorDescriber { get; }

        protected IUnitOfWork UnitOfWork { get; }

        public bool AutoSaveChanges { get; set; } = true;

        protected StoreBase(IdentityErrorDescriber describer, IUnitOfWork unitOfWork)
        {
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        protected virtual Task CoreSaveChangesAsync(CancellationToken cancellationToken)
        {
            return UnitOfWork.CommitAsync(cancellationToken);
        }

        protected virtual Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? CoreSaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        protected virtual async Task<IdentityResult> TrySaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Converts the provided <paramref name="value"/> to a strongly typed <see cref="long"/>.
        /// </summary>
        protected virtual long ConvertFromStringId(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                return 0L;

            if (!long.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                throw new ArgumentException("Input string was not in a correct format.", paramName);

            return result;
        }

        /// <summary>
        /// Converts the provided <paramref name="value"/> to its string representation.
        /// </summary>
        protected virtual string ConvertToStringId(long value)
        {
            return value == 0L ? null : value.ToString();
        }

    }
}