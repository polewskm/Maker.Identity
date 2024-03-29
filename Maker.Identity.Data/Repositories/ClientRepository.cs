﻿using Maker.Identity.Contracts.Entities;
using Maker.Identity.Contracts.Repositories;
using Maker.Identity.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Maker.Identity.Data.Repositories
{
    public class ClientRepository<TContext> : RepositoryEntityFramework<TContext, Client>, IClientRepository
        where TContext : DbContext
    {
        public ClientRepository(TContext context, IQueryBuilder queryBuilder)
            : base(context, queryBuilder)
        {
            // nothing
        }

    }
}