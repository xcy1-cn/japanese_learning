namespace JapaneseLearningApi.Responses;

public class SubmitAnswerResponse
{
    public bool IsCorrect { get; set; }

    public string CorrectAnswer { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;
}