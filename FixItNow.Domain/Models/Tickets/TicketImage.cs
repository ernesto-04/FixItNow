namespace FixItNow.Domain.Models.Tickets
{
    public class TicketImage
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string ImageUrl { get; set; }
    }
}