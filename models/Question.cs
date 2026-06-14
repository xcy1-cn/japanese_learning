namespace JapaneseLearningApi.Models;

public class Question
{
    public int Id { get; set; }

    public int? ArticleId { get; set; }

    public Article? Article { get; set; }

    public int? SentenceId { get; set; }

    public Sentence? Sentence { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Stem { get; set; } = string.Empty;

    public string OptionA { get; set; } = string.Empty;

    public string OptionB { get; set; } = string.Empty;

    public string OptionC { get; set; } = string.Empty;

    public string OptionD { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}