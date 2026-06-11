namespace JapaneseLearningApi.Responses;

public class PublicArticleSentencesResponse
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public string JapaneseText { get; set; } = string.Empty;

    public string ChineseText { get; set; } = string.Empty;

    public string Romaji { get; set; } = string.Empty;

    public int OrderIndex { get; set; }
}