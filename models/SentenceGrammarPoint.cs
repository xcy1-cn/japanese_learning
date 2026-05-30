namespace JapaneseLearningApi.Models;

public class SentenceGrammarPoint
{
    public int SentenceId { get; set; }

    public Sentence? Sentence { get; set; }

    public int GrammarPointId { get; set; }

    public GrammarPoint? GrammarPoint { get; set; }
}