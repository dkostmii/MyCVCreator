using Microsoft.AspNetCore.Mvc;
using MyCVCreator.Models;
using MyCVCreator.Services;
using System.Text;

namespace MyCVCreator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CVController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(
            [FromBody] CV cv,
            [FromServices] ILogger<CVController> logger,
            [FromServices] DocumentService service)
        {
            var html = service.CreateCV(
                new ModelSanitizer<CV>(cv).Sanitized);

            logger.LogInformation("Created document with CV {html}.", html);

            var timestamp = DateTime.Now.ToString("yyyy MM dd HH mm ss".Replace(" ", ""));

            return File(Encoding.UTF8.GetBytes(html), "text/html", $"MyCV-{timestamp}.html");
        }
    }
}
