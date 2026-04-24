using FixItNow.Domain.Models.DTOs;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure.Models.Commons;

namespace FixItNow.Application.Services
{
    public class TicketService
    {
        private readonly FixItNowDataContext _context;

        public TicketService(FixItNowDataContext context)
        {
            _context = context;
        }

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
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return new CreateTicketResponse
            {
                Id = ticket.Id
            };
        }

        public List<AvailableTicketResponse> GetAvailableTickets()
        {
            return _context.Tickets
                .Where(t => t.Status == TicketStatus.Unassigned)
                .Select(t => new AvailableTicketResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Category = t.Category,
                    Description = t.Description,
                    Location = t.Location,
                    TechnicianName = null,
                    Status = t.Status
                })
                .ToList();
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

            switch (ticket.Status)
            {
                case TicketStatus.Assigned:
                    if (newStatus != TicketStatus.InProgress)
                        throw new Exception("Can only move to InProgress");
                    break;

                case TicketStatus.InProgress:
                    if (newStatus != TicketStatus.Completed)
                        throw new Exception("Can only move to Completed");
                    break;

                case TicketStatus.Completed:
                    throw new Exception("Ticket already completed");
            }

            ticket.Status = newStatus;
            _context.SaveChanges();
        }
    }
}
