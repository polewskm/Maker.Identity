﻿using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class ClientSecretRepository<TContext> : RepositoryEntityFramework<TContext, ClientSecret>, IClientSecretRepository
        where TContext : DbContext
    {
        public ClientSecretRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}