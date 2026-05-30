namespace JapaneseLearningApi.Models;

public class Vocabulary
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public string Reading { get; set; } = string.Empty;

    public string Meaning { get; set; } = string.Empty;

    public string PartOfSpeech { get; set; } = string.Empty;

    public string ExampleSentence { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SentenceVocabulary> SentenceVocabularies { get; set; } = new();
}