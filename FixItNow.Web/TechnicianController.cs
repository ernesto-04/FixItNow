using FixItNow.Infrastructure.Models.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Web
{
    [Route("api/technicians")]
    [ApiController]
    public class TechnicianController : ControllerBase
    {
        private readonly FixItNowDataContext _context;

        public TechnicianController(FixItNowDataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTechnicians()
        {
            var technicians = _context.Technicians.ToList();
            return Ok(technicians);
        }
    }
}
