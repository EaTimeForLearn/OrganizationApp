using System.ComponentModel.DataAnnotations;

namespace Domain.Entities

{
    public class TicketCompany
    {
        [Key]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "İsim boş bırakılamaz."), MaxLength(20, ErrorMessage = "Fazla karakter girdiniz.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [MinLength(8, ErrorMessage = "En az 8 karakterli olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Şifre en az bir büyük , bir küçük, bir sayı, ve bir özel karakter içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mail boş bırakılamaz."), EmailAddress]
        public string Mail { get; set; }
        [MaxLength(100, ErrorMessage = "Adres fazla uzun.")]

        public string DomainAddress { get; set; }
        public decimal ProfitRate { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}
