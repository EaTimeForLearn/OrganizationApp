using Application.Repositories;
using Domain.Entities;
using Domain.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
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
    public class MemberAccountController : Controller
    {
        private readonly IMemberReadRepository _memberReadRepository;
        private readonly IMemberWriteRepository _memberWriteRepository;
        private IConfiguration _config;

        OrganizationDbContext _context;
        public MemberAccountController(OrganizationDbContext context, IConfiguration config, IMemberReadRepository memberReadRepository, IMemberWriteRepository memberWriteRepository)
        {
            _memberReadRepository = memberReadRepository;
            _memberWriteRepository = memberWriteRepository;
            _context = context;
            _config = config;   
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody]LoginDto loginDto)
        {
           
            var member = Authenticate(loginDto);
            if (member != null)
            {
                var token = Generate(member);
                return Ok(token);
            }

            return NotFound("Kullanıcı Bulunamadı");
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount(Member member)
        {
            Member _member = _context.Members.FirstOrDefault(a => a.Email == member.Email);      
                if (member.Role != null)
                    return BadRequest("Role bilgisi boş bırakınız.");
                else if (_member != null)
                    return BadRequest("Bu mail kullanılmaktadır.");
                else if(!ModelState.IsValid)
                return BadRequest("Validation kurallarına uyulmadı");
            
            member.Role = "User";
            await _memberWriteRepository.AddAsync(member);
            return Ok("Eklendi");
        }

        private string Generate(Member member)
        {
            var key = Encoding.UTF8.GetBytes(_config["TokenOption:SecretKey"]);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, member.Email));
            claims.Add(new Claim(ClaimTypes.Name, member.Name));
            claims.Add(new Claim(ClaimTypes.Role, member.Role));


            var token = new JwtSecurityToken(
                _config["TokenOption:Issuer"],
                _config["TokenOption:Audience"],
                claims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials); 
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
        private Member Authenticate(LoginDto loginDto)
        {
            var currentUser = _context.Members.FirstOrDefault(o => o.Email==loginDto.Email && o.Password == loginDto.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
