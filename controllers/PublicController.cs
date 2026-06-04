using JapaneseLearningApi.Data;
using JapaneseLearningApi.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    private readonly AppDbContext _context;

    public PublicController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles(
    string? keyword,
    string? level,
    string? category,
    int page = 1,
    int pageSize = 10
)
    {
        if (page <= 0)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var query = _context.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(keyword) ||
                a.Content.Contains(keyword)
            );
        }

        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(a => a.Level == level);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Level,
                a.Category,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    [HttpGet("articles/{id}")]
    public async Task<IActionResult> GetArticleDetail(int id)
    {
        var article = await _context.Articles
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Content,
                a.Level,
                a.Category,
                a.CreatedAt,
                a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (article == null)
        {
            return NotFound("Article not found.");
        }

        return Ok(article);
    }

    [HttpGet("articles/{id}/sentences")]
    public async Task<IActionResult> GetArticleSentences(int id)
    {
        var articleExists = await _context.Articles
            .AnyAsync(a => a.Id == id);

        if (!articleExists)
        {
            return NotFound("Article not found.");
        }

        var sentences = await _context.Sentences
            .Where(s => s.ArticleId == id)
            .OrderBy(s => s.OrderIndex)
            .Select(s => new
            {
                s.Id,
                s.ArticleId,
                s.JapaneseText,
                s.ChineseText,
                s.Romaji,
                s.OrderIndex
            })
            .ToListAsync();

        return Ok(sentences);
    }

    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestions(
    string? type,
    string? keyword,
    int page = 1,
    int pageSize = 10
)
    {
        if (page <= 0)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var query = _context.Questions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(q => q.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(q =>
                q.Stem.Contains(keyword) ||
                q.OptionA.Contains(keyword) ||
                q.OptionB.Contains(keyword) ||
                q.OptionC.Contains(keyword) ||
                q.OptionD.Contains(keyword)
            );
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(q => q.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new
            {
                q.Id,
                q.Type,
                q.Stem,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.ArticleId,
                q.SentenceId,
                q.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    [HttpGet("questions/{id}")]
    public async Task<IActionResult> GetQuestionDetail(int id)
    {
        var question = await _context.Questions
            .Where(q => q.Id == id)
            .Select(q => new
            {
                q.Id,
                q.Type,
                q.Stem,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.ArticleId,
                q.SentenceId,
                q.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (question == null)
        {
            return NotFound("Question not found.");
        }

        return Ok(question);
    }

    [HttpPost("questions/{id}/submit")]
    public async Task<IActionResult> SubmitAnswer(
    int id,
    SubmitAnswerRequest request
)
    {
        var question = await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
        {
            return NotFound("Question not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            return BadRequest("Answer is required.");
        }

        var userAnswer = request.Answer.Trim().ToUpper();
        var correctAnswer = question.Answer.Trim().ToUpper();

        var isCorrect = userAnswer == correctAnswer;

        return Ok(new
        {
            isCorrect,
            correctAnswer = question.Answer,
            explanation = question.Explanation
        });
    }
}