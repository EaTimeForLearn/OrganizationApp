using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DTOs
{
    public class UpdateEventDto
    {
        public int EventId { get; set; }
        public string Address { get; set; }

        public int Quota { get; set; }
    }
}
