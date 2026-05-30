namespace JapaneseLearningApi.Models;

public class SentenceVocabulary
{
    public int SentenceId { get; set; }

    public Sentence? Sentence { get; set; }

    public int VocabularyId { get; set; }

    public Vocabulary? Vocabulary { get; set; }
}