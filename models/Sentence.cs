namespace JapaneseLearningApi.Models;

public class Sentence
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public Article? Article { get; set; }

    public string JapaneseText { get; set; } = string.Empty;

    public string ChineseText { get; set; } = string.Empty;

    public string Romaji { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SentenceVocabulary> SentenceVocabularies { get; set; } = new();

    public List<SentenceGrammarPoint> SentenceGrammarPoints { get; set; } = new();

    public List<Question> Questions { get; set; } = new();
}