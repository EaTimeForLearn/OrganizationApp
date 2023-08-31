using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Domain.Entities

{
    public class Member 
    {
        [Key]
        public int MemberId { get; set; }
        [Required(ErrorMessage ="İsim boş bırakılamaz."),MaxLength(20,ErrorMessage ="Fazla karakter girdiniz.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyad boş bırakılamaz."),MaxLength(20, ErrorMessage = "Fazla karakter girdiniz.")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Mail boş bırakılamaz."),EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [MinLength(8, ErrorMessage = "En az 8 karakterli olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Şifre en az bir büyük , bir küçük, bir sayı, ve bir özel karakter içermelidir.")]
        public string Password { get; set; }
        public string? Role { get; set; }
        public List<Event> Events { get; set; }
    }
}
