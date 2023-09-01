using Application.Repositories;
using Domain.Entities;
using Domain.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using System.Security.Claims;

namespace OrganizationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Company")]

    public class TicketCompanyController : ControllerBase
    {
        OrganizationDbContext _context;
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly IEventWriteRepository _eventWriteRepository;
        private readonly IEventReadRepository _eventReadRepository;
        private readonly ITicketCompanyReadRepository _ticketCompanyReadRepository;
        private readonly ITicketCompanyWriteRepository _ticketCompanyWriteRepository;
        public TicketCompanyController(OrganizationDbContext context, ITicketCompanyReadRepository ticketCompanyReadRepository, ITicketCompanyWriteRepository ticketCompanyWriteRepository, IEventReadRepository eventReadRepository, IEventWriteRepository eventWriteRepository, ITicketReadRepository ticketReadRepository, ITicketWriteRepository ticketWriteRepository)
        {
            _ticketCompanyReadRepository = ticketCompanyReadRepository;
            _ticketCompanyWriteRepository = ticketCompanyWriteRepository;
            _context = context;
            _eventReadRepository = eventReadRepository;
            _eventWriteRepository = eventWriteRepository;
            _ticketReadRepository = ticketReadRepository;
            _ticketWriteRepository = ticketWriteRepository;
        }
        [HttpGet]
        public IActionResult EventList()
        {
            //List<Event> eventss = _context.Events.Where(e => e.Ticket == true).ToList();
            List<Event> events = _eventReadRepository.GetAll().Where(e => e.Ticket==true&&e.Status=="Onaylandi").ToList();
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] int id)
        {
           
            TicketCompany company = LoggedCompany();
            Event _event = FindEventById(id);
            Ticket _ticket= _context.Tickets.FirstOrDefault(m => m.EventId == id&&m.CompanyId==company.CompanyId);
           
            if (_event == null||_ticket!=null)
            return BadRequest("Böyle bir etkinlik mevcut değil veya zaten bilet oluşturulmuş."); 
            else if (_event.Ticket != true)
                return BadRequest("Bu etkinlik için bilet oluşturulamaz.");
            else
            {
            Ticket ticket = new Ticket()
            {
                Event = _event,
                CompanyId = company.CompanyId,
                EventId = _event.EventId,
                //Price = _event.TicketPrice * company.ProfitRate,
                Price = Math.Round(_event.TicketPrice * company.ProfitRate / (decimal)5.0) * (decimal)5.0,
                TicketCompany = company
            };
            await _ticketWriteRepository.AddAsync(ticket);
            return Ok($"{_event.EventName} etkinliğine bilet oluşturuldu");
            }
        }

        [HttpDelete()]
        public IActionResult DeleteTicket([FromBody] int id)
        {
            Ticket ticket = FindTicketById(id);  
            if(ticket==null)
                return NotFound("Doğru bilet id'si girdiğinize emin olun.");
            TicketCompany company = LoggedCompany();
            if (company.CompanyId != ticket.CompanyId)
                return NotFound("bilet bu şirkete ait değil.");
            _ticketWriteRepository.Remove(id);
            return Ok("Bilet Silindi.");
        }
        
        //şuan giriş yapmış olan şirketi bulan method.
        private TicketCompany LoggedCompany()
        {
            string mail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            TicketCompany company = _context.TicketCompanies.FirstOrDefault(m => m.Mail == mail);

            return company;
        }
        private Ticket FindTicketById(int id)
        {
            
            return _context.Tickets.Find(id); 
        }
        private Event FindEventById(int id)
        {
            Event _event = _context.Events.FirstOrDefault(a => a.EventId == id);

            return _event;
        }
    }
}
