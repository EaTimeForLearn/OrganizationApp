using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DTOs
{
    public class StatusDto
    {
        public int EventId { get; set; }
        public bool StatusApprove { get; set; }
    }
}
