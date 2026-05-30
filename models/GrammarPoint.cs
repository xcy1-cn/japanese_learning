namespace JapaneseLearningApi.Models;

public class GrammarPoint
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Structure { get; set; } = string.Empty;

    public string Example { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SentenceGrammarPoint> SentenceGrammarPoints { get; set; } = new();
}