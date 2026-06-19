// DTOs/Imports/ImportResultDto.cs
namespace JapaneseLearning.DTOs.Imports;

public class ImportResultDto
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = new();
}