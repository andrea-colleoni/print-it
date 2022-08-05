using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintIt.Core;

namespace PrintIt.ServiceHost.Controllers
{
    [ApiController]
    [Route("print")]
    public class PrintController : ControllerBase
    {
        private readonly IPdfPrintService _pdfPrintService;

        public PrintController(IPdfPrintService pdfPrintService)
        {
            _pdfPrintService = pdfPrintService;
        }

        [HttpPost]
        [Route("from-pdf")]
        public async Task<IActionResult> PrintFromPdf([FromForm] PrintFromTemplateRequest request)
        {
            await using Stream pdfStream = request.PdfFile.OpenReadStream();
            _pdfPrintService.Print(pdfStream, request.PrinterPath, request.PageRange);
            return Ok();
        }

        [HttpPost]
        [Route("from-file-path")]
        public async Task<IActionResult> PrintFromFilePath(string filePath, string printerPath, string pageRange = null)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            await using Stream pdfStream = System.IO.File.OpenRead(filePath);
            _pdfPrintService.Print(pdfStream, printerPath, pageRange);
            return Ok();
        }
    }

    public sealed class PrintFromTemplateRequest
    {
        [Required]
        public IFormFile PdfFile { get; set; }

        [Required]
        public string PrinterPath { get; set; }

        public string PageRange { get; set; }
    }
}
