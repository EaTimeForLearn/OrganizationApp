using System.ComponentModel.DataAnnotations;

namespace Domain.Entities

{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        public int CompanyId { get; set; }
        public int EventId { get; set; }
        public decimal Price { get; set; }
        public Event Event { get; set; }
        public TicketCompany TicketCompany { get; set; }

    }
}
