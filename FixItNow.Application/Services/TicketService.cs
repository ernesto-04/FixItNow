using FixItNow.Domain.Models.DTOs;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FixItNow.Application.Services
{
    public interface ITicketService
    {
        Task<CreateTicketResponse> CreateTicketAsync(int customerId, CreateTicketRequest request);
        List<AvailableTicketResponse> GetAvailableTickets(int userId);
        List<CustomerTicketResponse> GetCustomerTickets(int userId);
        List<TechnicianTicketResponse> GetTechnicianTickets(int userId);
        void AcceptTicket(int ticketId, int technicianUserId);
        void UpdateStatus(int ticketId, int userId, TicketStatus newStatus);
        Task<TicketChatResponse?> GetTicketChatAsync(int ticketId, int userId);
        Task<AvailableTicketResponse?> GetTicketForTechnicianAsync(int ticketId, int userId);
        Task<TechnicianTicketResponse?> GetTechnicianTicketDetailAsync(int ticketId, int userId);
        Task<CustomerTicketResponse?> GetCustomerTicketDetailAsync(int ticketId, int userId);
    }

    public class TicketService : ITicketService
    {
        private readonly FixItNowDataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public TicketService(FixItNowDataContext context, IWebHostEnvironment env, IConfiguration config)
        {
            _context = context;
            _env = env;
            _config = config;
        }

        private static readonly Dictionary<TicketStatus, List<TicketStatus>> _validTransitions =
        new()
        {
            { TicketStatus.Unassigned, new() { TicketStatus.Assigned } },
            { TicketStatus.Assigned, new() { TicketStatus.InProgress } },
            { TicketStatus.InProgress, new() { TicketStatus.Completed } },
            { TicketStatus.Completed, new() { } }
        };

        public async Task<CreateTicketResponse> CreateTicketAsync(int customerId, CreateTicketRequest request)
        {
            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Location = request.Location,
                CustomerId = customerId,
                Status = TicketStatus.Unassigned,
                AssignedTechnicianId = null,
                CreatedAt = DateTime.UtcNow,
                Images = new List<TicketImage>()
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // ✅ Handle images
            if (request.Images != null && request.Images.Any())
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "tickets");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var file in request.Images)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    // ✅ Save to DB
                    ticket.Images.Add(new TicketImage
                    {
                        TicketId = ticket.Id,
                        ImageUrl = $"/uploads/tickets/{fileName}"
                    });
                }

                await _context.SaveChangesAsync();
            }

            return new CreateTicketResponse
            {
                Id = ticket.Id
            };
        }

        public List<CustomerTicketResponse> GetCustomerTickets(int userId)
        {
            var baseUrl = _config["AppSettings:BaseUrl"];
            return _context.Tickets
                .Include(t => t.AssignedTechnician)
                .Include(t => t.Images)
                .Where(t => t.CustomerId == userId)
                .Select(t => new CustomerTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Location = t.Location,
                    Status = t.Status,
                    TechnicianName = t.AssignedTechnician != null
                        ? t.AssignedTechnician.Email
                        : "Not assigned",
                    AssignedTechnicianId = t.AssignedTechnicianId,
                    CustomerId = t.CustomerId,
                    ImageUrls = t.Images
                        .Select(i => $"{_config[baseUrl]}{i.ImageUrl}")
                        .ToList()
                })
                .ToList();
        }

        public async Task<AvailableTicketResponse?> GetTicketForTechnicianAsync(int ticketId, int userId)
        {
            var baseUrl = _config["AppSettings:BaseUrl"];

            var ticket = await _context.Tickets
                .Include(t => t.Images)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return null;

            return new AvailableTicketResponse
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Location = ticket.Location,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                CustomerName = ticket.Customer.Username,
                ImageUrls = ticket.Images
                .Select(i => $"{_config[baseUrl]}{i.ImageUrl}")
                .ToList()
            };
        }

        public List<AvailableTicketResponse> GetAvailableTickets(int currentUserId)
        {
            var baseUrl = _config["AppSettings:BaseUrl"];
            return _context.Tickets
                .Include(t => t.Images)
                .Where(t =>
            t.Status == TicketStatus.Unassigned
            && t.CustomerId != currentUserId
        )
                .Select(t => new AvailableTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Category = t.Category,
                    Description = t.Description,
                    Location = t.Location,
                    TechnicianName = null,
                    Status = t.Status,
                    ImageUrls = t.Images
                        .Select(i => $"{_config[baseUrl]}{i.ImageUrl}")
                        .ToList()
                })
                .ToList();
        }

        public List<TechnicianTicketResponse> GetTechnicianTickets(int userId)
        {
            var baseUrl = _config["AppSettings:BaseUrl"];
            return _context.Tickets
                .Include(t => t.Customer)
                .Include(t => t.Images)
                .Where(t => t.AssignedTechnicianId == userId)
                .Select(t => new TechnicianTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Location = t.Location,
                    Status = t.Status,
                    CustomerName = t.Customer.Email,
                    CustomerId = t.CustomerId,
                    AssignedTechnicianId = t.AssignedTechnicianId,
                    ImageUrls = t.Images
                        .Select(i => $"{_config[baseUrl]}{i.ImageUrl}")
                        .ToList()
                })
                .ToList();
        }

        public async Task<TechnicianTicketResponse?> GetTechnicianTicketDetailAsync(int ticketId, int userId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Images)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t =>
                    t.Id == ticketId &&
                    t.AssignedTechnicianId == userId);

            if (ticket == null)
                return null;

            return new TechnicianTicketResponse
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Location = ticket.Location,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                CustomerName = ticket.Customer.Username,
                ImageUrls = ticket.Images.Select(i => i.ImageUrl).ToList()
            };
        }

        public void AcceptTicket(int ticketId, int technicianUserId)
        {
            var ticket = _context.Tickets
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            //// ✅ Check ticket availability
            if (ticket.Status != TicketStatus.Unassigned)
                throw new Exception("Ticket is already taken");

            //// ✅ Check if user is a technician
            var isTechnician = _context.TechnicianProfiles
                .Any(tp => tp.UserId == technicianUserId);

            if (!isTechnician)
                throw new Exception("User is not a technician");

            // ✅ Assign ticket
            ticket.AssignedTechnicianId = technicianUserId;
            ticket.Status = TicketStatus.Assigned;

            _context.SaveChanges();
        }

        public void UpdateStatus(int ticketId, int userId, TicketStatus newStatus)
        {
            var ticket = _context.Tickets
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            if (ticket.AssignedTechnicianId != userId)
                throw new Exception("You are not assigned to this ticket");

            var currentStatus = ticket.Status;

            if (!_validTransitions.ContainsKey(currentStatus) ||
                !_validTransitions[currentStatus].Contains(newStatus))
            {
                throw new Exception($"Invalid status transition: {currentStatus} → {newStatus}");
            }

            ticket.Status = newStatus;
            _context.SaveChanges();
        }

        public async Task<TicketChatResponse?> GetTicketChatAsync(int ticketId, int userId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Images)
                .Include(t => t.AssignedTechnician)
                .Include(t => t.Customer)
               .FirstOrDefaultAsync(t =>
                    t.Id == ticketId &&
                    (
                        t.CustomerId == userId ||
                        t.AssignedTechnicianId == userId
                    ));

            if (ticket == null)
                return null;

            return new TicketChatResponse
            {
                Id = ticket.Id,

                CustomerId = ticket.CustomerId,

                AssignedTechnicianId =
            ticket.AssignedTechnicianId,

                TechnicianName =
            ticket.AssignedTechnician?.Email,

                CustomerName =
            ticket.Customer.Email,

                Status = ticket.Status,

                Title = ticket.Title,
                ReceiverId =
    ticket.CustomerId == userId
        ? ticket.AssignedTechnicianId ?? 0
        : ticket.CustomerId
            };
        }
        
        public async Task<CustomerTicketResponse?> GetCustomerTicketDetailAsync(
    int ticketId,
    int userId)
        {
            var baseUrl = _config["AppSettings:BaseUrl"];

            var ticket = await _context.Tickets
                .Include(t => t.Images)
                .Include(t => t.AssignedTechnician)
                .FirstOrDefaultAsync(t =>
                    t.Id == ticketId &&
                    (
                        t.CustomerId == userId ||
                        t.AssignedTechnicianId == userId
                    ));

            if (ticket == null)
                return null;

            return new CustomerTicketResponse
            {
                Id = ticket.Id,

                Title = ticket.Title,

                Description = ticket.Description,

                Category = ticket.Category,

                Location = ticket.Location,

                Status = ticket.Status,

                TechnicianName =
                    ticket.AssignedTechnician?.Email,

                AssignedTechnicianId =
                    ticket.AssignedTechnicianId,

                CustomerId = ticket.CustomerId,

                CreatedAt = ticket.CreatedAt,

                ImageUrls = ticket.Images
                    .Select(i => $"{_config[baseUrl]}{i.ImageUrl}")
                    .ToList()
            };
        }
    }
}
