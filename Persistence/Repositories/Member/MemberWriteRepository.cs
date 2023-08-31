using Domain.Entities;
using Persistence.Contexts;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class MemberWriteRepository : WriteRepository<Member>, IMemberWriteRepository
    {
        public MemberWriteRepository(OrganizationDbContext context) : base(context)
        {
        }
    }
}
