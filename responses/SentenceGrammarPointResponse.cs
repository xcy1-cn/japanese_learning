namespace JapaneseLearningApi.Responses;

public class SentenceGrammarPointResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string Structure { get; set; } = string.Empty;

    public string Example { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;
}