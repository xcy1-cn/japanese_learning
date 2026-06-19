// DTOs/Imports/ImportErrorDto.cs
namespace JapaneseLearning.DTOs.Imports;

public class ImportErrorDto
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}