using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using IdGen;
using Maker.Identity.Contracts.Audit;
using Maker.Identity.Contracts.Entities;
using Maker.Identity.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace Maker.Identity.Data
{
    /// <summary>
    /// Class for the Entity Framework database context used for identity.
    /// </summary>
    public class MakerDbContext : DbContext
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
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of audit change events.
        /// </summary>
        public DbSet<ChangeEvent> ChangeEvents { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of audit user events.
        /// </summary>
        public DbSet<UserEvent> UserEvents { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of user roles.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user claims.
        /// </summary>
        public DbSet<UserClaim> UserClaims { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user logins.
        /// </summary>
        public DbSet<UserLogin> UserLogins { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}" /> of user tokens.
        /// </summary>
        public DbSet<UserToken> UserTokens { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of user secrets.
        /// </summary>
        public DbSet<UserSecret> UserSecrets { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of roles.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of role claims.
        /// </summary>
        public DbSet<RoleClaim> RoleClaims { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{Secret}"/> of secrets.
        /// </summary>
        public DbSet<Secret> Secrets { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{SecretTag}"/> of secret tags.
        /// </summary>
        public DbSet<SecretTag> SecretTags { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{Client}"/> of clients.
        /// </summary>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{ClientTag}"/> of client tags.
        /// </summary>
        public DbSet<ClientTag> ClientTags { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{ClientSecret}"/> of client secrets.
        /// </summary>
        public DbSet<ClientSecret> ClientSecrets { get; set; }

        protected virtual void OnUserModelCreating(EntityTypeBuilder<User> entityBuilder) { }

        protected virtual void OnRoleModelCreating(EntityTypeBuilder<Role> entityBuilder) { }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string schemaName = "Identity";

            modelBuilder.Entity<ChangeEvent>(entityBuilder =>
            {
                entityBuilder.ToTable("ChangeEvents", schemaName).SkipChangeAudit();

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.PrincipalName, _.PrincipalKey, _.TimestampUtc }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.ChangeType).HasConversion<byte>();
                entityBuilder.Property(_ => _.UserName).HasMaxLength(256).IsUnicode(false);
                entityBuilder.Property(_ => _.PrincipalName).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.PrincipalKey).HasMaxLength(32).IsFixedLength().IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.KeyValues).IsRequired().IsUnicode(false);
            });

            modelBuilder.Entity<UserEvent>(entityBuilder =>
            {
                entityBuilder.ToTable("UserEvents", schemaName).SkipChangeAudit();

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.UserId, _.TimestampUtc });
                entityBuilder.HasIndex(_ => _.ActivityId);

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.EventType).HasConversion<int>().IsRequired();
                entityBuilder.Property(_ => _.ActivityId).HasMaxLength(50).IsUnicode(false);
                entityBuilder.Property(_ => _.AuthenticationMethod).HasMaxLength(50).IsUnicode(false);

                entityBuilder.HasOne<User>().WithMany().HasForeignKey(_ => _.UserId).IsRequired();
                entityBuilder.HasOne<Client>().WithMany().HasForeignKey(_ => _.ClientId);
            });

            modelBuilder.Entity<User>(entityBuilder =>
            {
                entityBuilder.ToTable("Users", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => _.NormalizedUserName).HasName("U_NormalizedUserName").IsUnique();
                entityBuilder.HasIndex(_ => _.NormalizedEmail).HasName("NU_NormalizedEmail");
                entityBuilder.HasIndex(_ => _.IsActive).HasName("NU_IsActive");

                entityBuilder.Property(_ => _.Id).UseIdGen();
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
            });


            modelBuilder.Entity<UserClaim>(entityBuilder =>
            {
                entityBuilder.ToTable("UserClaims", schemaName);
                entityBuilder.HasKey(_ => _.Id);

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.ClaimType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ClaimValue).IsRequired().IsUnicode(false);

                entityBuilder.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).IsRequired();
            });

            modelBuilder.Entity<UserLogin>(entityBuilder =>
            {
                entityBuilder.ToTable("UserLogins", schemaName).SkipChangeAudit();

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.LoginProvider, _.ProviderKey }).IsUnique();
                entityBuilder.HasIndex(_ => _.UserId);

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.LoginProvider).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ProviderKey).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ProviderDisplayName).HasMaxLength(256).IsRequired().IsUnicode();

                entityBuilder.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).IsRequired();
            });

            modelBuilder.Entity<UserToken>(entityBuilder =>
            {
                entityBuilder.ToTable("UserTokens", schemaName).SkipChangeAudit();

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.UserId, _.LoginProvider, _.Name }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.LoginProvider).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.Name).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.Value).IsRequired().IsUnicode(false);

                entityBuilder.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).IsRequired();
            });

            modelBuilder.Entity<UserSecret>(entityBuilder =>
            {
                entityBuilder.ToTable("UserSecrets", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.UserId, _.SecretId }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();

                entityBuilder.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).IsRequired();
                entityBuilder.HasOne(_ => _.Secret).WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            modelBuilder.Entity<Role>(entityBuilder =>
            {
                entityBuilder.ToTable("Roles", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => _.NormalizedName).HasName("U_NormalizedName").IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.Name).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.NormalizedName).HasMaxLength(256).IsRequired().IsUnicode(false);

                OnRoleModelCreating(entityBuilder);
            });

            modelBuilder.Entity<RoleClaim>(entityBuilder =>
            {
                entityBuilder.ToTable("RoleClaims", schemaName);
                entityBuilder.HasKey(_ => _.Id);

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.ClaimType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.ClaimValue).IsRequired().IsUnicode(false);

                entityBuilder.HasOne(_ => _.Role).WithMany().HasForeignKey(_ => _.RoleId).IsRequired();
            });

            modelBuilder.Entity<UserRole>(entityBuilder =>
            {
                entityBuilder.ToTable("UserRoles", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.UserId, _.RoleId }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();

                entityBuilder.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).IsRequired();
                entityBuilder.HasOne(_ => _.Role).WithMany().HasForeignKey(_ => _.RoleId).IsRequired();
            });

            modelBuilder.Entity<Secret>(entityBuilder =>
            {
                entityBuilder.ToTable("Secrets", schemaName);
                entityBuilder.HasKey(_ => _.Id);

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.CipherType).HasMaxLength(256).IsRequired().IsUnicode(false);
                entityBuilder.Property(_ => _.CipherText).IsRequired().IsUnicode(false);
            });

            modelBuilder.Entity<SecretTag>(entityBuilder =>
            {
                entityBuilder.AsTag("SecretTags", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.SecretId, _.NormalizedKey }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();

                entityBuilder.HasOne(_ => _.Secret).WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            modelBuilder.Entity<Client>(entityBuilder =>
            {
                entityBuilder.ToTable("Clients", schemaName);
                entityBuilder.HasKey(_ => _.Id);

                entityBuilder.Property(_ => _.Id).UseIdGen();
                entityBuilder.Property(_ => _.ConcurrencyStamp).IsConcurrencyStamp();

                entityBuilder.Property(_ => _.Disabled).IsRequired();
                entityBuilder.Property(_ => _.RequireSecret).IsRequired();
            });

            modelBuilder.Entity<ClientTag>(entityBuilder =>
            {
                entityBuilder.AsTag("ClientTags", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.ClientId, _.NormalizedKey }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();

                entityBuilder.HasOne(_ => _.Client).WithMany().HasForeignKey(_ => _.ClientId).IsRequired();
            });

            modelBuilder.Entity<ClientSecret>(entityBuilder =>
            {
                entityBuilder.ToTable("ClientSecrets", schemaName);

                entityBuilder.HasKey(_ => _.Id);
                entityBuilder.HasIndex(_ => new { _.ClientId, _.SecretId }).IsUnique();

                entityBuilder.Property(_ => _.Id).UseIdGen();

                entityBuilder.HasOne(_ => _.Client).WithMany().HasForeignKey(_ => _.ClientId).IsRequired();
                entityBuilder.HasOne(_ => _.Secret).WithMany().HasForeignKey(_ => _.SecretId).IsRequired();
            });

            modelBuilder.UseUtcDateTime();

            modelBuilder.UseIdGen(_idValueGenerator);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            int result;
            var timestamp = DateTime.UtcNow;

            IDbContextTransaction transaction = null;
            if (Database.CurrentTransaction == null && Database.GetEnlistedTransaction() == null && Transaction.Current == null)
            {
                transaction = Database.BeginTransaction();
            }

            using (transaction)
            {
                result = base.SaveChanges(false);

                var changes = ChangeTracker.Entries()
                    .Where(_ => _.ShouldAuditChanges())
                    .Select(entry => CreateAuditChangeEvent(timestamp, entry))
                    .ToList();

                ChangeTracker.AcceptAllChanges();

                ChangeEvents.AddRange(changes);

                result += base.SaveChanges(acceptAllChangesOnSuccess);

                transaction?.Commit();
            }

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            int result;
            var timestamp = DateTime.UtcNow;

            IDbContextTransaction transaction = null;
            if (Database.CurrentTransaction == null && Database.GetEnlistedTransaction() == null && Transaction.Current == null)
            {
                transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            }

            using (transaction)
            {
                result = await base.SaveChangesAsync(false, cancellationToken).ConfigureAwait(false);

                var changes = ChangeTracker.Entries()
                    .Where(_ => _.ShouldAuditChanges())
                    .Select(entry => CreateAuditChangeEvent(timestamp, entry))
                    .ToList();

                ChangeTracker.AcceptAllChanges();

                await ChangeEvents.AddRangeAsync(changes, cancellationToken).ConfigureAwait(false);

                result += await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);

                transaction?.Commit();
            }

            return result;
        }

        private static ChangeEvent CreateAuditChangeEvent(DateTime timestamp, EntityEntry entityEntry)
        {
            var keyValues = new Dictionary<string, object>();
            var oldValues = new Dictionary<string, object>();
            var newValues = new Dictionary<string, object>();

            var changeType = ChangeType.Unknown;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = ChangeType.Insert;
                    break;

                case EntityState.Modified:
                    changeType = ChangeType.Update;
                    break;

                case EntityState.Deleted:
                    changeType = ChangeType.Delete;
                    break;
            }

            var relational = entityEntry.Metadata.Relational();
            var schemaName = relational.Schema;
            var tableName = relational.TableName;
            var principalName = string.IsNullOrEmpty(schemaName) ? tableName : schemaName + "." + tableName;

            foreach (var property in entityEntry.Properties)
            {
                var propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    keyValues[propertyName] = property.CurrentValue;
                }

                if (property.Metadata.SkipChangeAudit())
                    continue;

                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        newValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Modified:
                        if (!property.IsModified) continue;
                        if (Equals(property.OriginalValue, property.CurrentValue)) continue;
                        oldValues[propertyName] = property.OriginalValue;
                        newValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        oldValues[propertyName] = property.OriginalValue;
                        break;
                }
            }

            var keyValuesJson = JsonConvert.SerializeObject(keyValues);

            byte[] principalKey;
            using (var hasher = SHA256.Create())
            {
                principalKey = hasher.ComputeHash(Encoding.UTF8.GetBytes(keyValuesJson));
            }

            return new ChangeEvent
            {
                TimestampUtc = timestamp,
                ChangeType = changeType,
                PrincipalName = principalName,
                PrincipalKey = principalKey,
                KeyValues = keyValuesJson,
                OldValues = oldValues.Count == 0 ? null : JsonConvert.SerializeObject(oldValues),
                NewValues = newValues.Count == 0 ? null : JsonConvert.SerializeObject(newValues),
            };
        }

    }
}