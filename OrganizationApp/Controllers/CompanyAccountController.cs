using Application.Repositories;
using Domain.Entities.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrganizationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyAccountController : ControllerBase
    {

        private readonly ITicketCompanyReadRepository _ticketCompanyReadRepository;
        private readonly ITicketCompanyWriteRepository _ticketCompanyWriteRepository;
        private IConfiguration _config;
        OrganizationDbContext _context;
        public CompanyAccountController(OrganizationDbContext context, IConfiguration config, ITicketCompanyReadRepository ticketCompanyReadRepository, ITicketCompanyWriteRepository ticketCompanyWriteRepository)
        {
            _ticketCompanyReadRepository = ticketCompanyReadRepository;
            _ticketCompanyWriteRepository = ticketCompanyWriteRepository;
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto loginDto)
        {

            TicketCompany company = Authenticate(loginDto);
            if (company != null)
            {
                var token = Generate(company);
                return Ok(token);
            }

            return NotFound("Kullanıcı Bulunamadı");
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount(TicketCompany company)
        {
            TicketCompany _company = _context.TicketCompanies.FirstOrDefault(a => a.Mail == company.Mail);

            if (!ModelState.IsValid || _company != null)
            {

                if (_company != null)
                    return BadRequest("Bu maile sahip kullanıcı bulunmaktadır.");
                else
                    return BadRequest("Yanlış ya da eksik bilgi girdiniz.");
            }

            await _ticketCompanyWriteRepository.AddAsync(company);
            return Ok("Eklendi");
        }

        private string Generate(TicketCompany company)
        {
            var key = Encoding.UTF8.GetBytes(_config["TokenOption:SecretKey"]);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, company.Mail));
            claims.Add(new Claim(ClaimTypes.Name, company.CompanyName));
            claims.Add(new Claim(ClaimTypes.Role, "Company"));

            var token = new JwtSecurityToken(
                _config["TokenOption:Issuer"],
                _config["TokenOption:Audience"],
                claims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
        private TicketCompany Authenticate(LoginDto loginDto)
        {
            var currentUser = _context.TicketCompanies.FirstOrDefault(o => o.Mail == loginDto.Email && o.Password == loginDto.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
