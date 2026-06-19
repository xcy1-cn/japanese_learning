using ClosedXML.Excel;
using JapaneseLearning.DTOs.Imports;
using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearning.Services;

public class ImportService : IImportService
{
    private readonly AppDbContext _context;

    public ImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ImportResultDto> ImportVocabulariesAsync(IFormFile file)
    {
        var result = new ImportResultDto();
        var vocabularies = new List<Vocabulary>();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            result.Errors.Add(new ImportErrorDto
            {
                RowNumber = 0,
                Field = "Sheet",
                Message = "Excel 中没有可读取的工作表。"
            });

            result.FailCount = 1;
            return result;
        }

        var expectedHeaders = new List<string>
        {
            "Word",
            "Reading",
            "Meaning",
            "PartOfSpeech",
            "Level"
        };

        var headerErrors = ValidateHeaders(worksheet, expectedHeaders);

        if (headerErrors.Any())
        {
            result.Errors.AddRange(headerErrors);
            result.FailCount = headerErrors.Count;
            return result;
        }

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            var word = GetCellValue(worksheet, row, 1);
            var reading = GetCellValue(worksheet, row, 2);
            var meaning = GetCellValue(worksheet, row, 3);
            var partOfSpeech = GetCellValue(worksheet, row, 4);
            var level = GetCellValue(worksheet, row, 5).ToUpper();

            if (IsEmptyRow(word, reading, meaning, partOfSpeech, level))
            {
                continue;
            }

            var rowErrors = ValidateVocabularyRow(row, word, meaning, level);

            if (rowErrors.Any())
            {
                result.Errors.AddRange(rowErrors);
                continue;
            }

            vocabularies.Add(new Vocabulary
            {
                Word = word,
                Reading = reading,
                Meaning = meaning,
                PartOfSpeech = partOfSpeech,
                Level = level,
                CreatedAt = DateTime.Now
            });
        }

        result.FailCount = result.Errors
            .Where(e => e.RowNumber > 1)
            .Select(e => e.RowNumber)
            .Distinct()
            .Count();

        result.SuccessCount = vocabularies.Count;

        if (!vocabularies.Any())
        {
            return result;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Vocabularies.AddRangeAsync(vocabularies);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // TODO:
            // 如果你已经有 Public 缓存版本号失效服务，可以在这里触发。
            // 例如：
            // _publicCacheVersionService.IncrementVersion();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return result;
    }

    public async Task<ImportResultDto> ImportGrammarPointsAsync(IFormFile file)
    {
        var result = new ImportResultDto();
        var grammarPoints = new List<GrammarPoint>();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            result.Errors.Add(new ImportErrorDto
            {
                RowNumber = 0,
                Field = "Sheet",
                Message = "Excel 中没有可读取的工作表。"
            });

            result.FailCount = 1;
            return result;
        }

        var expectedHeaders = new List<string>
        {
            "Title",
            "Explanation",
            "Structure",
            "Example",
            "Level"
        };

        var headerErrors = ValidateHeaders(worksheet, expectedHeaders);

        if (headerErrors.Any())
        {
            result.Errors.AddRange(headerErrors);
            result.FailCount = headerErrors.Count;
            return result;
        }

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            var title = GetCellValue(worksheet, row, 1);
            var explanation = GetCellValue(worksheet, row, 2);
            var structure = GetCellValue(worksheet, row, 3);
            var example = GetCellValue(worksheet, row, 4);
            var level = GetCellValue(worksheet, row, 5).ToUpper();

            if (IsEmptyRow(title, explanation, structure, example, level))
            {
                continue;
            }

            var rowErrors = ValidateGrammarPointRow(row, title, explanation, level);

            if (rowErrors.Any())
            {
                result.Errors.AddRange(rowErrors);
                continue;
            }

            grammarPoints.Add(new GrammarPoint
            {
                Title = title,
                Explanation = explanation,
                Structure = structure,
                Example = example,
                Level = level,
                CreatedAt = DateTime.Now
            });
        }

        result.FailCount = result.Errors
            .Where(e => e.RowNumber > 1)
            .Select(e => e.RowNumber)
            .Distinct()
            .Count();

        result.SuccessCount = grammarPoints.Count;

        if (!grammarPoints.Any())
        {
            return result;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.GrammarPoints.AddRangeAsync(grammarPoints);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // TODO: 导入成功后触发 Public 缓存失效
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return result;
    }

    public async Task<ImportResultDto> ImportQuestionsAsync(IFormFile file)
    {
        var result = new ImportResultDto();
        var questions = new List<Question>();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            result.Errors.Add(new ImportErrorDto
            {
                RowNumber = 0,
                Field = "Sheet",
                Message = "Excel 中没有可读取的工作表。"
            });

            result.FailCount = 1;
            return result;
        }

        var expectedHeaders = new List<string>
        {
            "ArticleId",
            "SentenceId",
            "Type",
            "Stem",
            "OptionA",
            "OptionB",
            "OptionC",
            "OptionD",
            "Answer",
            "Explanation",
            "Level"
        };

        var headerErrors = ValidateHeaders(worksheet, expectedHeaders);

        if (headerErrors.Any())
        {
            result.Errors.AddRange(headerErrors);
            result.FailCount = headerErrors.Count;
            return result;
        }

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            var articleIdText = GetCellValue(worksheet, row, 1);
            var sentenceIdText = GetCellValue(worksheet, row, 2);
            var type = GetCellValue(worksheet, row, 3);
            var stem = GetCellValue(worksheet, row, 4);
            var optionA = GetCellValue(worksheet, row, 5);
            var optionB = GetCellValue(worksheet, row, 6);
            var optionC = GetCellValue(worksheet, row, 7);
            var optionD = GetCellValue(worksheet, row, 8);
            var answer = GetCellValue(worksheet, row, 9).ToUpper();
            var explanation = GetCellValue(worksheet, row, 10);
            var level = GetCellValue(worksheet, row, 11).ToUpper();

            if (IsEmptyRow(
                    articleIdText,
                    sentenceIdText,
                    type,
                    stem,
                    optionA,
                    optionB,
                    optionC,
                    optionD,
                    answer,
                    explanation,
                    level))
            {
                continue;
            }

            var rowErrors = ValidateQuestionRow(
                row,
                articleIdText,
                sentenceIdText,
                type,
                stem,
                optionA,
                optionB,
                optionC,
                optionD,
                answer,
                level);

            if (rowErrors.Any())
            {
                result.Errors.AddRange(rowErrors);
                continue;
            }

            int? articleId = string.IsNullOrWhiteSpace(articleIdText)
                ? null
                : int.Parse(articleIdText);

            int? sentenceId = string.IsNullOrWhiteSpace(sentenceIdText)
                ? null
                : int.Parse(sentenceIdText);

            questions.Add(new Question
            {
                ArticleId = articleId,
                SentenceId = sentenceId,
                Type = type,
                Stem = stem,
                OptionA = optionA,
                OptionB = optionB,
                OptionC = optionC,
                OptionD = optionD,
                Answer = answer,
                Explanation = explanation,
                Level = level,
                CreatedAt = DateTime.Now
            });
        }

        result.FailCount = result.Errors
            .Where(e => e.RowNumber > 1)
            .Select(e => e.RowNumber)
            .Distinct()
            .Count();

        result.SuccessCount = questions.Count;

        if (!questions.Any())
        {
            return result;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Questions.AddRangeAsync(questions);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // TODO: 导入成功后触发 Public 缓存失效
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return result;
    }

    private List<ImportErrorDto> ValidateHeaders(
        IXLWorksheet worksheet,
        List<string> expectedHeaders)
    {
        var errors = new List<ImportErrorDto>();

        for (int i = 0; i < expectedHeaders.Count; i++)
        {
            var actualHeader = GetCellValue(worksheet, 1, i + 1);
            var expectedHeader = expectedHeaders[i];

            if (!string.Equals(actualHeader, expectedHeader, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new ImportErrorDto
                {
                    RowNumber = 1,
                    Field = expectedHeader,
                    Message = $"表头错误：第 {i + 1} 列应为 {expectedHeader}，当前为 {actualHeader}。"
                });
            }
        }

        return errors;
    }

    private List<ImportErrorDto> ValidateVocabularyRow(
        int row,
        string word,
        string meaning,
        string level)
    {
        var errors = new List<ImportErrorDto>();

        if (string.IsNullOrWhiteSpace(word))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Word",
                Message = "单词不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(meaning))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Meaning",
                Message = "中文含义不能为空。"
            });
        }

        if (!string.IsNullOrWhiteSpace(level) && !IsValidLevel(level))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Level",
                Message = "等级只能是 N5、N4、N3、N2、N1。"
            });
        }

        return errors;
    }

    private List<ImportErrorDto> ValidateGrammarPointRow(
        int row,
        string title,
        string explanation,
        string level)
    {
        var errors = new List<ImportErrorDto>();

        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Title",
                Message = "语法标题不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(explanation))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Explanation",
                Message = "语法解释不能为空。"
            });
        }

        if (!string.IsNullOrWhiteSpace(level) && !IsValidLevel(level))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Level",
                Message = "等级只能是 N5、N4、N3、N2、N1。"
            });
        }

        return errors;
    }

    private List<ImportErrorDto> ValidateQuestionRow(
        int row,
        string articleIdText,
        string sentenceIdText,
        string type,
        string stem,
        string optionA,
        string optionB,
        string optionC,
        string optionD,
        string answer,
        string level)
    {
        var errors = new List<ImportErrorDto>();

        if (!string.IsNullOrWhiteSpace(articleIdText) && !int.TryParse(articleIdText, out _))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "ArticleId",
                Message = "ArticleId 必须是数字。"
            });
        }

        if (!string.IsNullOrWhiteSpace(sentenceIdText) && !int.TryParse(sentenceIdText, out _))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "SentenceId",
                Message = "SentenceId 必须是数字。"
            });
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Type",
                Message = "题目类型不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(stem))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Stem",
                Message = "题干不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(optionA))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "OptionA",
                Message = "选项 A 不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(optionB))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "OptionB",
                Message = "选项 B 不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(optionC))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "OptionC",
                Message = "选项 C 不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(optionD))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "OptionD",
                Message = "选项 D 不能为空。"
            });
        }

        if (string.IsNullOrWhiteSpace(answer))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Answer",
                Message = "正确答案不能为空。"
            });
        }
        else if (!IsValidAnswer(answer))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Answer",
                Message = "正确答案只能是 A、B、C、D。"
            });
        }

        if (!string.IsNullOrWhiteSpace(level) && !IsValidLevel(level))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = row,
                Field = "Level",
                Message = "等级只能是 N5、N4、N3、N2、N1。"
            });
        }

        return errors;
    }

    private string GetCellValue(IXLWorksheet worksheet, int row, int column)
    {
        return worksheet.Cell(row, column).GetString().Trim();
    }

    private bool IsEmptyRow(params string[] values)
    {
        return values.All(string.IsNullOrWhiteSpace);
    }

    private bool IsValidLevel(string level)
    {
        var validLevels = new HashSet<string>
        {
            "N5",
            "N4",
            "N3",
            "N2",
            "N1"
        };

        return validLevels.Contains(level.ToUpper());
    }

    private bool IsValidAnswer(string answer)
    {
        var validAnswers = new HashSet<string>
        {
            "A",
            "B",
            "C",
            "D"
        };

        return validAnswers.Contains(answer.ToUpper());
    }
}