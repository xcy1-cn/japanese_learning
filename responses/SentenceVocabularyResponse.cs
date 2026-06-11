namespace JapaneseLearningApi.Responses;

public class SentenceVocabularyResponse
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public string Reading { get; set; } = string.Empty;

    public string Meaning { get; set; } = string.Empty;

    public string PartOfSpeech { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;
}