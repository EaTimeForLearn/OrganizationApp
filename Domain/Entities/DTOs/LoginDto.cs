using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Mail boş bırakılamaz."), EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [MinLength(8, ErrorMessage = "En az 8 karakterli olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Şifre en az bir büyük , bir küçük, bir sayı, ve bir özel karakter içermelidir.")]
        public string Password { get; set; }

    }
}
