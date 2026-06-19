// Services/IImportService.cs
using JapaneseLearning.DTOs.Imports;

namespace JapaneseLearning.Services;

public interface IImportService
{
    Task<ImportResultDto> ImportVocabulariesAsync(IFormFile file);
    Task<ImportResultDto> ImportGrammarPointsAsync(IFormFile file);
    Task<ImportResultDto> ImportQuestionsAsync(IFormFile file);
}