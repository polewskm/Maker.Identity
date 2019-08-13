using System;
using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts;
using Maker.Identity.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data
{
    public class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        public TContext Context { get; }

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public IChangeEventRepository ChangeEventRepository { get; }
        public IUserRepository UserRepository { get; }
        public IUserClaimRepository UserClaimRepository { get; }
        public IUserLoginRepository UserLoginRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IUserSecretRepository UserSecretRepository { get; }
        public IUserTokenRepository UserTokenRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IRoleClaimRepository RoleClaimRepository { get; }
        public IClientRepository ClientRepository { get; }
        public IClientTagRepository ClientTagRepository { get; }
        public IClientSecretRepository ClientSecretRepository { get; }
        public ISecretRepository SecretRepository { get; }
        public ISecretTagRepository SecretTagRepository { get; }
    }
}