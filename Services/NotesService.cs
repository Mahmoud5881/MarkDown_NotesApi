using System.Text.Json;
using Markdig;
using NotesApi.Interfaces;

namespace NotesApi.Services;

public class NotesService : INotesService
{
    private static readonly HttpClient client = new();
    
    public async Task UploadAsync(IFormFile file)
    {
        var path = Path.Combine("Files", file.FileName);
        await using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    }

    public async Task<JsonElement> CheckAsync(IFormFile file)
    {
        var path = Path.Combine("Files", file.FileName);
        using var reader = new StreamReader(file.OpenReadStream());
        var text = await reader.ReadToEndAsync();
        var values = new Dictionary<string, string>()
        {
            { "text", text },
            { "language", "en-US" }
        };
        HttpContent content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("https://api.languagetool.org/v2/check", content);
        var newContent = await response.Content.ReadAsStringAsync();
        var jsonObj = JsonDocument.Parse(newContent).RootElement;
        return jsonObj;
    }

    public async Task SaveAsync(string text)
    {
        var path = Path.Combine("Files", $"note_{Guid.NewGuid()}.md");
        
        await File.WriteAllTextAsync(path, text);
    }

    public async Task<List<string>> GetAsync()
    {
        List<string> names = new List<string>();

        foreach (var file in Directory.EnumerateFiles("Files"))
        {
            names.Add(Path.GetFileNameWithoutExtension(file));
        }

        return names;
    }

    public async Task<string> RenderAsync(IFormFile file)
    {
        var path = Path.Combine("Files", file.FileName);
        using var reader = new StreamReader(file.OpenReadStream());
        var text = await reader.ReadToEndAsync();
        return Markdown.ToHtml(text);
    }
}