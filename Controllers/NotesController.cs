using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Interfaces;

namespace NotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }
        
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0) 
            {
                await _notesService.UploadAsync(file);
                return Ok();
            }
            return BadRequest(new { message = "no file found" });
        }

        [HttpPost("Check-grammar")]
        public async Task<IActionResult> CheckGrammar(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var content = await _notesService.CheckAsync(file);
                return Ok(content);
            }
            return BadRequest(new { message = "no file found" });
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody]string text)
        {
            if (text != null && ModelState.IsValid)
            {
                await _notesService.SaveAsync(text);
                return Created();
            }
            ModelState.AddModelError("text", "Text is Invalid.");
            return BadRequest(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var files = await _notesService.GetAsync();
            if (files != null)
            {
                return Ok(files);
            }
            return BadRequest( new { message = "Notes not found." });
        }

        [HttpPost]
        public async Task<IActionResult> Render(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var htmlText = await _notesService.RenderAsync(file);
                return Ok(htmlText);
            }
            return BadRequest(new { message = "no file found" });
        }
    }
}
