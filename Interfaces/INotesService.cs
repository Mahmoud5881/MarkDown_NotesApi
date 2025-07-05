using System.Text.Json;

namespace NotesApi.Interfaces;

public interface INotesService
{
    Task UploadAsync(IFormFile file);

    Task<JsonElement> CheckAsync(IFormFile file);

    Task SaveAsync(string text);

    Task<List<string>> GetAsync();

    Task<string> RenderAsync(IFormFile file);
}