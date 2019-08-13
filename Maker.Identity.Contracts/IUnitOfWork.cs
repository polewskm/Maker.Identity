using System.Threading;
using System.Threading.Tasks;
using Maker.Identity.Contracts.Repositories;

namespace Maker.Identity.Contracts
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken cancellationToken = default);

        IChangeEventRepository ChangeEventRepository { get; }

        IUserRepository UserRepository { get; }

        IUserClaimRepository UserClaimRepository { get; }

        IUserLoginRepository UserLoginRepository { get; }

        IUserRoleRepository UserRoleRepository { get; }

        IUserSecretRepository UserSecretRepository { get; }

        IUserTokenRepository UserTokenRepository { get; }

        IRoleRepository RoleRepository { get; }

        IRoleClaimRepository RoleClaimRepository { get; }

        IClientRepository ClientRepository { get; }

        IClientTagRepository ClientTagRepository { get; }

        IClientSecretRepository ClientSecretRepository { get; }

        ISecretRepository SecretRepository { get; }

        ISecretTagRepository SecretTagRepository { get; }
    }
}