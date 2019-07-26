using IdGen;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maker.Identity.Stores
{
    public interface IRoleDbContext<TRole, TRoleBase, TRoleHistory>
        where TRole : class, TRoleBase, ISupportConcurrencyToken
        where TRoleBase : class, IRoleBase, ISupportAssign<TRoleBase>
        where TRoleHistory : class, TRoleBase, IHistoryEntity<TRoleBase>
    {
        DbSet<TRole> Roles { get; set; }

        DbSet<TRoleHistory> RoleHistory { get; set; }

        DbSet<RoleClaim> RoleClaims { get; set; }

        DbSet<RoleClaimHistory> RoleClaimHistory { get; set; }
    }

    public interface IUserDbContext<TUser, TUserBase, TUserHistory>
        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>
    {
        DbSet<TUser> Users { get; set; }

        DbSet<TUserHistory> UserHistory { get; set; }

        DbSet<UserClaim> UserClaims { get; set; }

        DbSet<UserClaimHistory> UserClaimHistory { get; set; }

        DbSet<UserLogin> UserLogins { get; set; }

        DbSet<UserToken> UserTokens { get; set; }
    }

    public interface IUserRoleDbContext<TUser, TUserBase, TUserHistory, TRole, TRoleBase, TRoleHistory> :
        IUserDbContext<TUser, TUserBase, TUserHistory>,
        IRoleDbContext<TRole, TRoleBase, TRoleHistory>

        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>

        where TRole : class, TRoleBase, ISupportConcurrencyToken
        where TRoleBase : class, IRoleBase, ISupportAssign<TRoleBase>
        where TRoleHistory : class, TRoleBase, IHistoryEntity<TRoleBase>
    {
        DbSet<UserRole> UserRoles { get; set; }

        DbSet<UserRoleHistory> UserRoleHistory { get; set; }
    }

    public interface IUserSecretDbContext<TUser, TUserBase, TUserHistory> :
        IUserDbContext<TUser, TUserBase, TUserHistory>,
        ISecretDbContext

        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>
    {
        DbSet<UserSecret> UserSecrets { get; set; }

        DbSet<UserSecretHistory> UserSecretHistory { get; set; }
    }

    public interface ISecretDbContext
    {
        DbSet<Secret> Secrets { get; set; }

        DbSet<SecretHistory> SecretHistory { get; set; }

        DbSet<SecretTag> SecretTags { get; set; }

        DbSet<SecretTagHistory> SecretTagHistory { get; set; }
    }

    public interface IClientDbContext
    {
        DbSet<Client> Clients { get; set; }

        DbSet<ClientHistory> ClientHistory { get; set; }

        DbSet<ClientTag> ClientTags { get; set; }

        DbSet<ClientTagHistory> ClientTagHistory { get; set; }
    }

    public interface IClientSecretDbContext : IClientDbContext, ISecretDbContext
    {
        DbSet<ClientSecret> ClientSecrets { get; set; }

        DbSet<ClientSecretHistory> ClientSecretHistory { get; set; }
    }

    public class MakerDbContext : MakerDbContext<User, UserBase, UserHistory, Role, RoleBase, RoleHistory>
    {
        public MakerDbContext(DbContextOptions options, IIdGenerator<long> idGenerator)
            : base(options, idGenerator)
        {
            // nothing
        }
    }

    /// <summary>
    /// Class for the Entity Framework database context used for identity.
    /// </summary>
    public class MakerDbContext<TUser, TUserBase, TUserHistory, TRole, TRoleBase, TRoleHistory> : DbContext,
        IUserRoleDbContext<TUser, TUserBase, TUserHistory, TRole, TRoleBase, TRoleHistory>,
        IUserSecretDbContext<TUser, TUserBase, TUserHistory>,
        IClientSecretDbContext

        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>

        where TRole : class, TRoleBase, ISupportConcurrencyToken
        where TRoleBase : class, IRoleBase, ISupportAssign<TRoleBase>
        where TRoleHistory : class, TRoleBase, IHistoryEntity<TRoleBase>
    {
        private readonly IdValueGenerator _idValueGenerator;

        /// <summary>
        /// Initializes a new instance of <see cref="MakerDbContext"/>.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        /// <param name="idGenerator"></param>
        public MakerDbContext(DbContextOptions options, IIdGenerator<long> idGenerator)
            : base(options)
        {
            _idValueGenerator = new IdValueGenerator(idGenerator);
        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of users.
        /// </summary>
        public DbSet<TUser> Users { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user history.
        /// </summary>
        public DbSet<TUserHistory> UserHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user claims.
        /// </summary>
        public DbSet<UserClaim> UserClaims { get; set; }

        public DbSet<UserClaimHistory> UserClaimHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user logins.
        /// </summary>
        public DbSet<UserLogin> UserLogins { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user tokens.
        /// </summary>
        public DbSet<UserToken> UserTokens { get; set; }

        //

        public DbSet<UserSecret> UserSecrets { get; set; }

        public DbSet<UserSecretHistory> UserSecretHistory { get; set; }

        //

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of roles.
        /// </summary>
        public DbSet<TRole> Roles { get; set; }

        public DbSet<TRoleHistory> RoleHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of user roles.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<UserRoleHistory> UserRoleHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of role claims.
        /// </summary>
        public DbSet<RoleClaim> RoleClaims { get; set; }

        public DbSet<RoleClaimHistory> RoleClaimHistory { get; set; }

        //

        /// <summary>
        /// Gets or sets the <see cref="DbSet{Secret}"/> of secrets.
        /// </summary>
        public DbSet<Secret> Secrets { get; set; }

        public DbSet<SecretHistory> SecretHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{SecretTag}"/> of secret tags.
        /// </summary>
        public DbSet<SecretTag> SecretTags { get; set; }

        public DbSet<SecretTagHistory> SecretTagHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{Client}"/> of clients.
        /// </summary>
        public DbSet<Client> Clients { get; set; }

        public DbSet<ClientHistory> ClientHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{ClientTag}"/> of client tags.
        /// </summary>
        public DbSet<ClientTag> ClientTags { get; set; }

        public DbSet<ClientTagHistory> ClientTagHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{ClientSecret}"/> of client secrets.
        /// </summary>
        public DbSet<ClientSecret> ClientSecrets { get; set; }

        public DbSet<ClientSecretHistory> ClientSecretHistory { get; set; }

        protected virtual void OnUserModelCreating(EntityTypeBuilder<TUser> entityBuilder) { }

        protected virtual void OnUserHistoryModelCreating(EntityTypeBuilder<TUserHistory> entityBuilder) { }

        protected virtual void OnRoleModelCreating(EntityTypeBuilder<TRole> entityBuilder) { }

        protected virtual void OnRoleHistoryModelCreating(EntityTypeBuilder<TRoleHistory> entityBuilder) { }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string schemaName = "Identity";

            modelBuilder.EntityWithHistory<TUser, TUserBase, TUserHistory>("UserHistory", entityBuilder =>
            {
                entityBuilder.ToTable("Users", schemaName);

                entityBuilder.HasKey(_ => _.UserId);
                entityBuilder.HasIndex(_ => _.NormalizedUserName).HasName("U_NormalizedUserName").IsUnique();
                entityBuilder.HasIndex(_ => _.NormalizedEmail).HasName("NU_NormalizedEmail");
                entityBuilder.HasIndex(_ => _.IsActive).HasName("NU_IsActive");

                entityBuilder.Property(_ => _.UserId).UseIdGen();
                entityBuilder.Property(_ => _.IsActive).IsRequired();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.FirstName).HasMaxLength(256).IsRequired().IsUnicode();
                entityBuilder.Property(_ => _.LastName).HasMaxLength(256).IsRequired().IsUnicode();

                entityBuilder.Property(_ => _.UserName).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.NormalizedUserName).HasMaxLength(256).IsRequired().IsUnicode(false);

                entityBuilder.Property(_ => _.Email).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.NormalizedEmail).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.EmailConfirmed).IsRequired();

                entityBuilder.Property(_ => _.PasswordHash).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.SecurityStamp).HasMaxLength(50).IsRequired().IsUnicode(false);

                entityBuilder.Property(_ => _.PhoneNumber).HasMaxLength(20).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.PhoneNumberConfirmed).IsRequired();

                entityBuilder.Property(_ => _.TwoFactorEnabled).IsRequired();
                entityBuilder.Property(_ => _.LockoutEndUtc);
                entityBuilder.Property(_ => _.LockoutEnabled).IsRequired();
                entityBuilder.Property(_ => _.AccessFailedCount).IsRequired();

                OnUserModelCreating(entityBuilder);

            }, OnUserHistoryModelCreating);


            modelBuilder.EntityWithHistory<UserClaim, UserClaimBase, UserClaimHistory>("UserClaimHistory", entityBuilder =>
            {
                entityBuilder.ToTable("UserClaims", schemaName);
                entityBuilder.HasKey(_ => _.UserClaimId);

                entityBuilder.Property(_ => _.UserClaimId).UseIdGen();
                entityBuilder.Property(_ => _.ClaimType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ClaimValue).IsRequired().IsUnicode(false);

                entityBuilder.HasOne<TUser>().WithMany().HasForeignKey(_ => _.UserId).IsRequired();
            });

            modelBuilder.Entity<UserLogin>(entityBuilder =>
            {
                entityBuilder.ToTable("UserLogins", schemaName);
                entityBuilder.HasKey(_ => new { _.LoginProvider, _.ProviderKey });

                entityBuilder.Property(_ => _.LoginProvider).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ProviderKey).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ProviderDisplayName).HasMaxLength(256).IsRequired().IsUnicode();

                entityBuilder.HasOne<TUser>().WithMany().HasForeignKey(_ => _.UserId).IsRequired();
            });

            modelBuilder.Entity<UserToken>(entityBuilder =>
            {
                entityBuilder.ToTable("UserTokens", schemaName);
                entityBuilder.HasKey(_ => new { _.UserId, _.LoginProvider, _.Name });

                entityBuilder.Property(_ => _.LoginProvider).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.Name).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.Value).IsRequired().IsUnicode(false);

                entityBuilder.HasOne<TUser>().WithMany().HasForeignKey(_ => _.UserId).IsRequired();
            });

            modelBuilder.EntityWithHistory<UserSecret, UserSecretBase, UserSecretHistory>("UserSecretHistory", entityBuilder =>
            {
                entityBuilder.ToTable("UserSecrets", schemaName);
                entityBuilder.HasKey(_ => new { _.UserId, _.SecretId });

                entityBuilder.HasOne<TUser>().WithMany().HasForeignKey(_ => _.UserId).IsRequired();
                entityBuilder.HasOne(_ => _.Secret).WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            modelBuilder.EntityWithHistory<TRole, TRoleBase, TRoleHistory>("RoleHistory", entityBuilder =>
            {
                entityBuilder.ToTable("Roles", schemaName);

                entityBuilder.HasKey(_ => _.RoleId);
                entityBuilder.HasIndex(_ => _.NormalizedName).HasName("U_NormalizedName").IsUnique();

                entityBuilder.Property(_ => _.RoleId).UseIdGen();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.Name).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.NormalizedName).HasMaxLength(256).IsRequired().IsUnicode(false);

                OnRoleModelCreating(entityBuilder);

            }, OnRoleHistoryModelCreating);

            modelBuilder.EntityWithHistory<RoleClaim, RoleClaimBase, RoleClaimHistory>("RoleClaimHistory", entityBuilder =>
            {
                entityBuilder.ToTable("RoleClaims", schemaName);
                entityBuilder.HasKey(_ => _.RoleClaimId);

                entityBuilder.Property(_ => _.RoleClaimId).UseIdGen();
                entityBuilder.Property(_ => _.ClaimType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ClaimValue).IsRequired().IsUnicode(false);

                entityBuilder.HasOne<TRole>().WithMany().HasForeignKey(_ => _.RoleId).IsRequired();
            });

            modelBuilder.EntityWithHistory<UserRole, UserRoleBase, UserRoleHistory>("UserRoleHistory", entityBuilder =>
            {
                entityBuilder.ToTable("UserRoles", schemaName);
                entityBuilder.HasKey(_ => new { _.UserId, _.RoleId });

                entityBuilder.HasOne<TUser>().WithMany().HasForeignKey(_ => _.UserId).IsRequired();
                entityBuilder.HasOne<TRole>().WithMany().HasForeignKey(_ => _.RoleId).IsRequired();
            });

            modelBuilder.EntityWithHistory<Secret, SecretBase, SecretHistory>("SecretHistory", entityBuilder =>
            {
                entityBuilder.ToTable("Secrets", schemaName);
                entityBuilder.HasKey(_ => _.SecretId);

                entityBuilder.Property(_ => _.SecretId).UseIdGen();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.CipherType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.CipherText).IsRequired().IsUnicode(false);
            });

            modelBuilder.EntityWithHistory<SecretTag, SecretTagBase, SecretTagHistory>("SecretTagHistory", entityBuilder =>
            {
                entityBuilder.AsTag("SecretTags", schemaName);
                entityBuilder.HasKey(_ => new { _.SecretId, _.NormalizedKey });

                entityBuilder.HasOne<Secret>().WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            modelBuilder.EntityWithHistory<Client, ClientBase, ClientHistory>("ClientHistory", entityBuilder =>
            {
                entityBuilder.ToTable("Clients", schemaName);
                entityBuilder.HasKey(_ => _.ClientId);

                entityBuilder.Property(_ => _.ClientId).UseIdGen();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.Disabled).IsRequired();
                entityBuilder.Property(_ => _.RequireSecret).IsRequired();
            });

            modelBuilder.EntityWithHistory<ClientTag, ClientTagBase, ClientTagHistory>("ClientTagHistory", entityBuilder =>
            {
                entityBuilder.AsTag("ClientTags", schemaName);
                entityBuilder.HasKey(_ => new { _.ClientId, _.NormalizedKey });

                entityBuilder.HasOne<Client>().WithMany().HasForeignKey(_ => _.ClientId).IsRequired();
            });

            modelBuilder.Entity<ClientSecret>(entityBuilder =>
            {
                entityBuilder.ToTable("ClientSecrets", schemaName);
                entityBuilder.HasKey(_ => new { _.ClientId, _.SecretId });

                entityBuilder.HasOne<Client>().WithMany().HasForeignKey(_ => _.ClientId).IsRequired();
                entityBuilder.HasOne<Secret>().WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            modelBuilder.UseIdGen(_idValueGenerator);
        }

    }
}