using Application.Repositories;
using Domain.Entities;
using Domain.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Persistence.Contexts;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrganizationApp.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class MemberController : ControllerBase
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly IEventParticipantReadRepository _eventParticipantReadRepository;
        private readonly IEventParticipantWriteRepository _eventParticipantWriteRepository;
        private readonly OrganizationDbContext _context;
        private readonly IMemberReadRepository _memberReadRepository;
        private readonly IMemberWriteRepository _memberWriteRepository;
        private readonly IEventWriteRepository _eventWriteRepository;
        private readonly IEventReadRepository _eventReadRepository;
        private readonly ITicketCompanyReadRepository _ticketCompanyReadRepository;

        public MemberController(IMemberReadRepository memberReadRepository, IMemberWriteRepository memberWriteRepository, IEventReadRepository eventReadRepository, IEventWriteRepository eventWriteRepository, IEventParticipantReadRepository eventParticipantReadRepository, IEventParticipantWriteRepository eventParticipantWriteRepository, ITicketReadRepository ticketReadRepository, ITicketWriteRepository ticketWriteRepository, ITicketCompanyReadRepository ticketCompanyReadRepository, OrganizationDbContext context)
        {
            _context = context;
            _memberReadRepository = memberReadRepository;
            _memberWriteRepository = memberWriteRepository;
            _eventReadRepository = eventReadRepository;
            _eventWriteRepository = eventWriteRepository;
            _eventParticipantReadRepository = eventParticipantReadRepository;
            _eventParticipantWriteRepository = eventParticipantWriteRepository;
            _ticketReadRepository = ticketReadRepository;
            _ticketWriteRepository = ticketWriteRepository;
            _ticketCompanyReadRepository = ticketCompanyReadRepository; 
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetMemberList()
        {
            IQueryable<Member> model = _memberReadRepository.GetAll();
            return Ok(model);
        }
        [HttpGet("getcompanies")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCompaniesList()
        {
            IQueryable<TicketCompany> companies = _ticketCompanyReadRepository.GetAll();
            return Ok(companies);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(int id)
        {
            Member member = await _memberReadRepository.GetByIdAsync(id);

            return Ok(member);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getEventsA")]
        public IActionResult GetEventList()
        {
            IQueryable<Event> model = _eventReadRepository.GetAll();

            return Ok(model);
        }

        [HttpGet("getEvents")]
        public IActionResult GetApprovedEventList()
        {
            IQueryable<Event> model = _eventReadRepository.GetAll().Where(a => a.Status == "Onaylandi");

            return Ok(model);
        }
        //[HttpGet("getEvents")]
        //public IActionResult GetEventList()
        //{
        //    //IQueryable<Event> model= _eventReadRepository.GetAll();
        //    Member member = LoggedMember();
        //    IQueryable<Event> model;
        //    switch (member.Role)
        //    {
        //        case "User":
        //            model = _eventReadRepository.GetAll().Where(a => a.Status == "Onaylandi");
        //            //model = _context.Events.Where(a => a.Status == "Onaylandi");
        //            break;

        //        default:
        //            model = _eventReadRepository.GetAll();
        //            break;
        //    }

        //    return Ok(model);
        //}

        [HttpGet("getTickets")]
        public IActionResult GetTicketList()
        {
            IQueryable<Ticket> model = _ticketReadRepository.GetAll();
            return Ok(model);
        }

        [HttpPost("addEvent")]
        public async Task<IActionResult> AddEvent(Event eventt)
        {
            try
            {

                Member member = LoggedMember();
                // Event üyesinin bilgilerini ata ve veritabanına ekle
                eventt.Member = member;
                eventt.Status = "Beklemede";
                await _eventWriteRepository.AddAsync(eventt);
                return Ok($"{eventt.EventName} etkinliği oluşturuldu.");


            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }

        [HttpPost("attendEvent")]
        public async Task<IActionResult> AttendEvent([FromBody] int id)
        {
            Event _event = FindEventById(id);

            if (_event == null||_event.Status=="Beklemede")
                return NotFound("Böyle bir etkinlik bulunamadı");
            int attendscount = AttendsCount(_event.EventId);
            if (_event.Ticket == true)
            {
                IQueryable<Ticket> model = _context.Tickets.Where(e => e.EventId == id);
                if (model == null)
                    return NotFound($"{_event.EventName} etkinliği için bilet bulunmamaktadır");
                return Ok(model);

            }
            else if ((_event.Quota - attendscount) <= 0)
            {
                return BadRequest("Kontenjan dolu");
            }
            Member member = LoggedMember();

            EventParticipant participant = new EventParticipant()
            {
                EventId = id,
                MemberId = member.MemberId,
                Surname = member.Surname,
                Name = member.Name,
                Member=member
            };

            await _eventParticipantWriteRepository.AddAsync(participant);
            return Ok($"{_event.EventName} etkinliğine katıldınız");
        }


        [HttpPost("BuyTicket")]
        public async Task<IActionResult> BuyTicket([FromBody] int id)
        {
            Ticket ticket = _context.Tickets.Find(id);
            if (ticket == null) 
                return NotFound("Böyle bir bilet bulunamadı.");
            TicketCompany company = _context.TicketCompanies.Find(ticket.CompanyId);
            Event _event=FindEventById(ticket.EventId);
            Member member = LoggedMember();
            int attendscount =AttendsCount(_event.EventId);
            if (_event == null || _event.Status == "Beklemede")
                return NotFound("Böyle bir etkinlik bulunamadı.");
            else if ((_event.Quota - attendscount) <= 0)
            {
                return BadRequest("Kontenjan dolu.");
            }
            else
            {
            EventParticipant participant = new EventParticipant()
            {
                EventId = ticket.EventId,
                Name = member.Name,
                Surname = member.Surname,
                MemberId = member.MemberId,
                
            };
            await _eventParticipantWriteRepository.AddAsync(participant);
            return Ok($"{company.DomainAddress} sitesine yönlendirildin");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteMember(int id)
        {

           bool _return= _memberWriteRepository.Remove(id);
            if(_return==true)
            return Ok();
            return NotFound("kişi bulunaadı");
        }

        [HttpDelete("deleteEvent,{id}")]
        public IActionResult DeleteEvent([FromRoute] int id)
        {
            Event _event = FindEventById(id);
            Member member = LoggedMember();
            if (_event != null && member.MemberId==_event.MemberId)
            {
                if ((_event.LastApplyTime - DateTime.Now).Days > 5)
                {
                    _eventWriteRepository.Remove(_event.EventId);
                    return Ok($"{_event.EventName} silindi.");
                }

                return BadRequest("Son 5 gun kala etkinlik silinemez.");
            }
            return NotFound("Etkinlik bulunamadı.");
        }

        [HttpPut]
        public IActionResult UpdateMember(Member member)
        {
            Member updated = LoggedMember();
            
            if (member == null || member.Role != null || !ModelState.IsValid)
            {
                if (member.Role != null)
                {
                    return BadRequest("Role bilgisi güncellenemez.");
                }
                return BadRequest("");
            }
           else if (member.MemberId!=updated.MemberId)
            {
            return NotFound($"{updated.Name} üyesine ait MemberId bilgisini doğru girdiğinizden emin olun.");
            }
            updated.Name = member.Name;
            updated.Email = member.Email;
            updated.Surname = member.Surname;
            updated.Password = member.Password;

            _memberWriteRepository.Update(updated);
            return Ok($"{updated.Name} üyesinin bilgileri güncellendi.");
        }

        [HttpPatch("approve")]
        [Authorize("Admin")]
        public IActionResult ApproveEvent(StatusDto status)
        {
            Event updated = FindEventById(status.EventId);
            if (updated.Status == "Onaylandi")
                return BadRequest("Zaten onaylanmış");
            else if (status.StatusApprove == true)
            {
                updated.Status = "Onaylandi";
                _eventWriteRepository.Update(updated);
                return Ok($"{updated.EventName} admin tarafından onaylandı.");
            }


            updated.Status = "Onaylanmadı";
            return DeleteEvent(status.EventId);
        }

        [HttpPut("updateEvent")]
        public IActionResult UpdateEvent(UpdateEventDto _event)
        {

            Member member = LoggedMember();

            Event updatedEvent = member.Events.FirstOrDefault(a => a.EventId == _event.EventId);
            if (updatedEvent == null)
            {
                return NotFound("Değiştirilcek etkinliğin EventId'sini doğru girdiğinizden emin olun.");
            }
            else if ((updatedEvent.EventDate - DateTime.Now).Days <= 5)
                return BadRequest("Etkinliğe 5 gun kala değiştiremezsiniz.");

            updatedEvent.Quota = _event.Quota;
            updatedEvent.Address = _event.Address;

            _eventWriteRepository.Update(updatedEvent);
            return Ok($"{updatedEvent.EventName} güncellendi");
        }

        //LoginMember methodu şuanki giriş yapmış kullanıcıyı bize buluyor.
        private Member LoggedMember( )
        {
            string memberEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Member member = _context.Members.FirstOrDefault(m => m.Email == memberEmail);
         
            return  member;
        }
        //FindEventById methodu , gönderilen id'li event'ı getiriyor.
        private Event FindEventById(int id)
        {
            Event _event = _context.Events.FirstOrDefault(a => a.EventId ==id);

            return _event;
        }

        //Etkinliğe katılanların sayısı
        private int AttendsCount(int EventId)
        {
            int attendscount = _context.EventParticipants.Where(a => a.EventId ==EventId).Count();

            return attendscount;
        }

    }
}
