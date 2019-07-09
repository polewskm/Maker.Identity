using IdGen;
using Maker.Identity.Stores.Entities;
using Maker.Identity.Stores.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Stores
{
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

        DbSet<UserLoginHistory> UserLoginHistory { get; set; }

        DbSet<UserToken> UserTokens { get; set; }

        DbSet<UserTokenHistory> UserTokenHistory { get; set; }

        //

        DbSet<Role> Roles { get; set; }

        DbSet<RoleHistory> RoleHistory { get; set; }

        DbSet<UserRole> UserRoles { get; set; }

        DbSet<UserRoleHistory> UserRoleHistory { get; set; }
    }

    public class MakerDbContext : MakerDbContext<User, UserBase, UserHistory>
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
    public class MakerDbContext<TUser, TUserBase, TUserHistory> : DbContext, IUserDbContext<TUser, TUserBase, TUserHistory>
        where TUser : class, TUserBase, ISupportConcurrencyToken
        where TUserBase : class, IUserBase, ISupportAssign<TUserBase>
        where TUserHistory : class, TUserBase, IHistoryEntity<TUserBase>
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

        public DbSet<UserLoginHistory> UserLoginHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user tokens.
        /// </summary>
        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<UserTokenHistory> UserTokenHistory { get; set; }

        //

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of roles.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        public DbSet<RoleHistory> RoleHistory { get; set; }

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

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            const string schemaName = "Identity";

            builder.EntityWithHistory<TUser, TUserBase, TUserHistory>("UserHistory", schemaName, entity =>
            {
                entity.ToTable("Users", schemaName);

                entity.HasKey(_ => _.UserId);
                entity.HasIndex(_ => _.NormalizedUserName).HasName("U_NormalizedUserName").IsUnique();
                entity.HasIndex(_ => _.NormalizedEmail).HasName("U_NormalizedEmail");

                entity.Property(_ => _.UserId).UseIdGen();
                entity.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entity.Property(_ => _.FirstName).HasMaxLength(256).IsRequired().IsUnicode();
                entity.Property(_ => _.LastName).HasMaxLength(256).IsRequired().IsUnicode();

                entity.Property(_ => _.UserName).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.NormalizedUserName).HasMaxLength(256).IsRequired().IsUnicode(false);

                entity.Property(_ => _.Email).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.NormalizedEmail).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.EmailConfirmed).IsRequired();

                entity.Property(_ => _.PasswordHash).IsRequired().IsUnicode(false);
                entity.Property(_ => _.SecurityStamp).HasMaxLength(50).IsRequired().IsUnicode(false);

                entity.Property(_ => _.PhoneNumber).HasMaxLength(20).IsRequired().IsUnicode(false);
                entity.Property(_ => _.PhoneNumberConfirmed).IsRequired();

                entity.Property(_ => _.TwoFactorEnabled).IsRequired();
                entity.Property(_ => _.LockoutEnd).IsRequired();
                entity.Property(_ => _.LockoutEnabled).IsRequired();
                entity.Property(_ => _.AccessFailedCount).IsRequired();

                entity.Property(_ => _.MembershipCreatedWhen).IsRequired();
                entity.Property(_ => _.MembershipExpiresWhen).IsRequired();

                entity.HasMany<UserClaim>().WithOne().HasForeignKey(_ => _.UserId).IsRequired();
                entity.HasMany<UserLogin>().WithOne().HasForeignKey(_ => _.UserId).IsRequired();
                entity.HasMany<UserToken>().WithOne().HasForeignKey(_ => _.UserId).IsRequired();
                entity.HasMany<UserRole>().WithOne().HasForeignKey(_ => _.UserId).IsRequired();
            });

            builder.EntityWithHistory<UserClaim, UserClaimBase, UserClaimHistory>("UserClaimHistory", schemaName, entity =>
            {
                entity.ToTable("UserClaims", schemaName);
                entity.HasKey(_ => _.UserClaimId);

                entity.Property(_ => _.UserClaimId).UseIdGen();
                entity.Property(_ => _.ClaimType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.ClaimValue).IsRequired().IsUnicode(false);
            });

            builder.EntityWithHistory<UserLogin, UserLoginBase, UserLoginHistory>("UserLoginHistory", schemaName, entity =>
            {
                entity.ToTable("UserLogins", schemaName);
                entity.HasKey(_ => new { _.LoginProvider, _.ProviderKey });

                entity.Property(_ => _.LoginProvider).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.ProviderKey).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.ProviderDisplayName).HasMaxLength(256).IsRequired().IsUnicode();
            });

            builder.EntityWithHistory<UserToken, UserTokenBase, UserTokenHistory>("UserTokenHistory", schemaName, entity =>
            {
                entity.ToTable("UserTokens", schemaName);
                entity.HasKey(_ => new { _.UserId, _.LoginProvider, _.Name });

                entity.Property(_ => _.LoginProvider).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.Name).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.Value).IsRequired().IsUnicode(false);
            });

            builder.EntityWithHistory<Role, RoleBase, RoleHistory>("RoleHistory", schemaName, entity =>
            {
                entity.ToTable("Roles", schemaName);

                entity.HasKey(_ => _.RoleId);
                entity.HasIndex(_ => _.NormalizedName).HasName("U_NormalizedName").IsUnique();

                entity.Property(_ => _.RoleId).UseIdGen();
                entity.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entity.Property(_ => _.Name).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.NormalizedName).HasMaxLength(256).IsRequired().IsUnicode(false);

                entity.HasMany<UserRole>().WithOne().HasForeignKey(_ => _.RoleId).IsRequired();
                entity.HasMany<RoleClaim>().WithOne().HasForeignKey(_ => _.RoleId).IsRequired();
            });

            builder.EntityWithHistory<RoleClaim, RoleClaimBase, RoleClaimHistory>("RoleClaimHistory", schemaName, entity =>
            {
                entity.ToTable("RoleClaims", schemaName);
                entity.HasKey(_ => _.RoleClaimId);

                entity.Property(_ => _.RoleClaimId).UseIdGen();
                entity.Property(_ => _.ClaimType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.ClaimValue).IsRequired().IsUnicode(false);
            });

            builder.EntityWithHistory<UserRole, UserRoleBase, UserRoleHistory>("UserRoleHistory", schemaName, entity =>
            {
                entity.ToTable("UserRoles", schemaName);
                entity.HasKey(_ => new { _.UserId, _.RoleId });
            });

            builder.EntityWithHistory<Secret, SecretBase, SecretHistory>("SecretHistory", schemaName, entity =>
            {
                entity.ToTable("Secrets", schemaName);
                entity.HasKey(_ => _.SecretId);

                entity.Property(_ => _.SecretId).UseIdGen();
                entity.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entity.Property(_ => _.CipherType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entity.Property(_ => _.CipherText).IsRequired().IsUnicode(false);
            });

            builder.EntityWithHistory<SecretTag, SecretTagBase, SecretTagHistory>("SecretTagHistory", schemaName, entity =>
            {
                entity.AsTag("SecretTags", schemaName);
                entity.HasKey(_ => new { _.SecretId, _.NormalizedKey });
                entity.HasOne<Secret>().WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            builder.EntityWithHistory<Client, ClientBase, ClientHistory>("ClientHistory", schemaName, entity =>
            {
                entity.ToTable("Clients", schemaName);
                entity.HasKey(_ => _.ClientId);

                entity.Property(_ => _.ClientId).UseIdGen();
                entity.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entity.Property(_ => _.Disabled).IsRequired();
                entity.Property(_ => _.RequireSecret).IsRequired();
            });

            builder.EntityWithHistory<ClientTag, ClientTagBase, ClientTagHistory>("ClientTagHistory", schemaName, entity =>
            {
                entity.AsTag("ClientTags", schemaName);
                entity.HasKey(_ => new { _.ClientId, _.NormalizedKey });
                entity.HasOne<Client>().WithMany().HasForeignKey(_ => _.ClientId).IsRequired();
            });

            builder.Entity<ClientSecret>(entity =>
            {
                entity.ToTable("ClientSecrets", schemaName);
                entity.HasKey(_ => new { _.ClientId, _.SecretId });

                entity.HasOne<Client>().WithMany().HasForeignKey(_ => _.ClientId).IsRequired();
                entity.HasOne<Secret>().WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            builder.UseIdGen(_idValueGenerator);
        }

    }
}