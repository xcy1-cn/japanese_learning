namespace JapaneseLearningApi.Models;

public class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List<Sentence> Sentences { get; set; } = new();
}