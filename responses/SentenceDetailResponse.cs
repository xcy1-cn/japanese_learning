namespace JapaneseLearningApi.Responses;

public class SentenceDetailResponse
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public string JapaneseText { get; set; } = string.Empty;

    public string ChineseText { get; set; } = string.Empty;

    public string? Romaji { get; set; }

    public int OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<SentenceVocabularyResponse> Vocabularies { get; set; } = new();

    public List<SentenceGrammarPointResponse> GrammarPoints { get; set; } = new();
}