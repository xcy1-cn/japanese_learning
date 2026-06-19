// Controllers/ImportController.cs
using JapaneseLearning.DTOs.Imports;
using JapaneseLearning.Services;
using JapaneseLearningApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JapaneseLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImportController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("vocabularies")]
    public async Task<IActionResult> ImportVocabularies(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            // return BadRequest(ApiResponse<ImportResultDto>.Fail(400,"Excel file is required."));
            return BadRequest(ApiResponse<string>.Fail(400, "Excel file is required."));
        }

        var result = await _importService.ImportVocabulariesAsync(file);

        return Ok(ApiResponse<ImportResultDto>.Success(result, "Vocabulary import completed."));
    }

    [HttpPost("grammar-points")]
    public async Task<IActionResult> ImportGrammarPoints(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            // return BadRequest(ApiResponse<ImportResultDto>.Fail(400, "Excel file is required."));
            return BadRequest(ApiResponse<string>.Fail(400, "Excel file is required."));
        }

        var result = await _importService.ImportGrammarPointsAsync(file);

        return Ok(ApiResponse<ImportResultDto>.Success(result, "Grammar point import completed."));
    }

    [HttpPost("questions")]
    public async Task<IActionResult> ImportQuestions(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            // return BadRequest(ApiResponse<ImportResultDto>.Fail(400, "Excel file is required."));
            return BadRequest(ApiResponse<string>.Fail(400, "Excel file is required."));
        }

        var result = await _importService.ImportQuestionsAsync(file);

        return Ok(ApiResponse<ImportResultDto>.Success(result, "Question import completed."));
    }
}