using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EventParticipant
    {
        [Key]
        public int ParticipantId { get; set; }
        public int MemberId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Event>? Events { get; set; }
        public Member Member { get; set; }

    }
}
