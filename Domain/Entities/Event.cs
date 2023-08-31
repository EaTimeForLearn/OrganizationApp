
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public int MemberId { get; set; }
        [Required,MaxLength(50,ErrorMessage ="Etkinlik ismi fazla uzun.")]
        public string EventName { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [MaxLength(100, ErrorMessage = "Adres fazla uzun.")]

        public string Address { get; set; }
        
        public int Quota { get; set; }
        public DateTime LastApplyTime { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public bool Ticket { get; set; }
        public decimal TicketPrice { get; set; }
        public string? Status { get; set; }

        public Member Member { get; set; }
        public List<EventParticipant>? Participants { get; set; }

    }
}
