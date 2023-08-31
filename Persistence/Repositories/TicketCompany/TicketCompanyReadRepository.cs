﻿using Domain.Entities;
using Persistence.Contexts;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class TicketCompanyReadRepository : ReadRepository<TicketCompany>, ITicketCompanyReadRepository
    {
        public TicketCompanyReadRepository(OrganizationDbContext context) : base(context)
        {
        }
    }
}
