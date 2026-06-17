using JapaneseLearningApi.Data;
using JapaneseLearningApi.Requests;
using JapaneseLearningApi.Responses;
using JapaneseLearningApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan PublicCacheDuration = TimeSpan.FromMinutes(5);

    private int GetArticlesCacheVersion()
    {
        return _cache.GetOrCreate(PublicCacheKeys.ArticlesVersion, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
            return 1;
        });
    }

    private int GetQuestionsCacheVersion()
    {
        return _cache.GetOrCreate(PublicCacheKeys.QuestionsVersion, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
            return 1;
        });
    }
    public PublicController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
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

        var version = GetArticlesCacheVersion();

        var cacheKey = PublicCacheKeys.ArticleList(
            keyword,
            level,
            category,
            page,
            pageSize,
            version
        );

        if (_cache.TryGetValue(cacheKey, out PagedResult<PublicArticleResponse>? cachedResult))
        {
            return Ok(ApiResponse<PagedResult<PublicArticleResponse>>.Success(
                cachedResult!,
                "Success from memory cache."
            ));
        }

        var query = _context.Articles
            .AsNoTracking()
            .AsQueryable();

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
            .Select(a => new PublicArticleResponse
            {
                Id = a.Id,
                Title = a.Title,
                Level = a.Level,
                Category = a.Category,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<PublicArticleResponse>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };

        _cache.Set(cacheKey, result, PublicCacheDuration);

        return Ok(ApiResponse<PagedResult<PublicArticleResponse>>.Success(result));
    }

    [HttpGet("articles/{id}")]
    public async Task<IActionResult> GetArticleDetail(int id)
    {
        var version = GetArticlesCacheVersion();

        var cacheKey = PublicCacheKeys.ArticleDetail(id, version);

        if (_cache.TryGetValue(cacheKey, out PublicArticleDetailResponse? cachedArticle))
        {
            return Ok(ApiResponse<PublicArticleDetailResponse>.Success(
                cachedArticle!,
                "Success from memory cache."
            ));
        }

        var article = await _context.Articles
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new PublicArticleDetailResponse
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Level = a.Level,
                Category = a.Category,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (article == null)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Article not found."));
        }

        _cache.Set(cacheKey, article, PublicCacheDuration);

        return Ok(ApiResponse<PublicArticleDetailResponse>.Success(article));
    }

    [HttpGet("articles/{id}/sentences")]
    public async Task<IActionResult> GetArticleSentences(int id)
    {
        var version = GetArticlesCacheVersion();

        var cacheKey = PublicCacheKeys.ArticleSentences(id, version);

        if (_cache.TryGetValue(cacheKey, out List<PublicArticleSentencesResponse>? cachedSentences))
        {
            return Ok(ApiResponse<List<PublicArticleSentencesResponse>>.Success(
                cachedSentences!,
                "Success from memory cache."
            ));
        }

        var articleExists = await _context.Articles
            .AsNoTracking()
            .AnyAsync(a => a.Id == id);

        if (!articleExists)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Article not found."));
        }

        var sentences = await _context.Sentences
            .AsNoTracking()
            .Where(s => s.ArticleId == id)
            .OrderBy(s => s.OrderIndex)
            .Select(s => new PublicArticleSentencesResponse
            {
                Id = s.Id,
                ArticleId = s.ArticleId,
                JapaneseText = s.JapaneseText,
                ChineseText = s.ChineseText,
                Romaji = s.Romaji,
                OrderIndex = s.OrderIndex
            })
            .ToListAsync();

        _cache.Set(cacheKey, sentences, PublicCacheDuration);

        return Ok(ApiResponse<List<PublicArticleSentencesResponse>>.Success(sentences));
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

        var version = GetQuestionsCacheVersion();

        var cacheKey = PublicCacheKeys.QuestionList(
            type,
            keyword,
            page,
            pageSize,
            version
        );

        if (_cache.TryGetValue(cacheKey, out PagedResult<QuestionDetailResponse>? cachedResult))
        {
            return Ok(ApiResponse<PagedResult<QuestionDetailResponse>>.Success(
                cachedResult!,
                "Success from memory cache."
            ));
        }

        var query = _context.Questions
            .AsNoTracking()
            .AsQueryable();

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
            .Select(q => new QuestionDetailResponse
            {
                Id = q.Id,
                Type = q.Type,
                Stem = q.Stem,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                ArticleId = q.ArticleId,
                SentenceId = q.SentenceId,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<QuestionDetailResponse>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };

        _cache.Set(cacheKey, result, PublicCacheDuration);

        return Ok(ApiResponse<PagedResult<QuestionDetailResponse>>.Success(result));
    }

    [HttpGet("questions/{id}")]
    public async Task<IActionResult> GetQuestionDetail(int id)
    {
        var version = GetQuestionsCacheVersion();

        var cacheKey = PublicCacheKeys.QuestionDetail(id, version);

        if (_cache.TryGetValue(cacheKey, out QuestionDetailResponse? cachedQuestion))
        {
            return Ok(ApiResponse<QuestionDetailResponse>.Success(
                cachedQuestion!,
                "Success from memory cache."
            ));
        }

        var question = await _context.Questions
            .AsNoTracking()
            .Where(q => q.Id == id)
            .Select(q => new QuestionDetailResponse
            {
                Id = q.Id,
                Type = q.Type,
                Stem = q.Stem,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                ArticleId = q.ArticleId,
                SentenceId = q.SentenceId,
                CreatedAt = q.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (question == null)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Question not found."));
        }

        _cache.Set(cacheKey, question, PublicCacheDuration);

        return Ok(ApiResponse<QuestionDetailResponse>.Success(question));
    }

    [HttpPost("questions/{id}/submit")]
    public async Task<IActionResult> SubmitAnswer(
    int id,
    SubmitAnswerRequest request
)
    {
        /* 
        AsNoTracking(): 这些 Public GET 都只是查询，不需要 EF Core 跟踪实体变化
         */
        var question = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
        {
            // return NotFound("Question not found.");
            return BadRequest(ApiResponse<string>.Fail(404, "Question not found."));
        }

        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            // return BadRequest("Answer is required.");
            return BadRequest(ApiResponse<string>.Fail(400, "Answer is required."));
        }

        var userAnswer = request.Answer.Trim().ToUpper();
        var correctAnswer = question.Answer.Trim().ToUpper();

        var isCorrect = userAnswer == correctAnswer;

        var result = new SubmitAnswerResponse
        {
            IsCorrect = isCorrect,
            CorrectAnswer = question.Answer,
            Explanation = question.Explanation
        };

        return Ok(ApiResponse<SubmitAnswerResponse>.Success(result));
    }
}