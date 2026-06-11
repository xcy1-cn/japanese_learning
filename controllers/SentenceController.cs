using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using JapaneseLearningApi.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SentenceController : ControllerBase
{
    private readonly AppDbContext _context;

    public SentenceController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Sentence
    [HttpGet]
    /* 
    string? keyword,
    int? articleId,
    GET /api/Sentence?keyword=学生&page=1&pageSize=10
     */
    public async Task<IActionResult> GetSentences(
    string? keyword,
    int? articleId,
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

        var query = _context.Sentences.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s =>
                s.JapaneseText.Contains(keyword) ||
                s.ChineseText.Contains(keyword) ||
                s.Romaji.Contains(keyword)
            );
        }

        if (articleId.HasValue)
        {
            query = query.Where(s => s.ArticleId == articleId.Value);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.OrderIndex)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PagedResult<Sentence>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };

        // return Ok(new
        // {
        //     total,
        //     page,
        //     pageSize,
        //     items
        // });

        return Ok(ApiResponse<PagedResult<Sentence>>.Success(result));
    }

    // GET: api/Sentence/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Sentence>> GetSentence(int id)
    {
        var sentence = await _context.Sentences
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sentence == null)
        {
            return BadRequest(ApiResponse<string>.Fail(404, "This sentence is not exist."));
        }

        // return Ok(sentence);
        return Ok(ApiResponse<Sentence>.Success(sentence));
    }

    //  GET: api/Sentence/1/detail
    [HttpGet("{id}/detail")]
    public async Task<IActionResult> GetSentenceDetail(int id)
    {
        var sentence = await _context.Sentences
            .Where(s => s.Id == id)
            .Select(s => new SentenceDetailResponse
            {
                Id = s.Id,
                ArticleId = s.ArticleId,
                JapaneseText = s.JapaneseText,
                ChineseText = s.ChineseText,
                Romaji = s.Romaji,
                OrderIndex = s.OrderIndex,
                CreatedAt = s.CreatedAt,

                Vocabularies = s.SentenceVocabularies.Select(sv => new SentenceVocabularyResponse
                {
                    Id = sv.Vocabulary!.Id,
                    Word = sv.Vocabulary.Word,
                    Reading = sv.Vocabulary.Reading,
                    Meaning = sv.Vocabulary.Meaning,
                    PartOfSpeech = sv.Vocabulary.PartOfSpeech,
                    Level = sv.Vocabulary.Level
                }).ToList(),

                GrammarPoints = s.SentenceGrammarPoints.Select(sg => new SentenceGrammarPointResponse
                {
                    Id = sg.GrammarPoint!.Id,
                    Title = sg.GrammarPoint.Title,
                    Explanation = sg.GrammarPoint.Explanation,
                    Structure = sg.GrammarPoint.Structure,
                    Example = sg.GrammarPoint.Example,
                    Level = sg.GrammarPoint.Level
                }).ToList()
            })
.FirstOrDefaultAsync();

        if (sentence == null)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Article not found."));
        }

        // return Ok(sentence);
        return Ok(ApiResponse<SentenceDetailResponse>.Success(sentence));
    }

    //  POST: api/Sentence/1/vocabularies/{vocabularyId}
    [HttpPost("{sentenceId}/vocabularies/{vocabularyId}")]
    public async Task<IActionResult> AddVocabularyToSentence(
    int sentenceId,
    int vocabularyId
)
    {
        var sentenceExists = await _context.Sentences
            .AnyAsync(s => s.Id == sentenceId);

        if (!sentenceExists)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Sentence not found."));
        }

        var vocabularyExists = await _context.Vocabularies
            .AnyAsync(v => v.Id == vocabularyId);

        if (!vocabularyExists)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Vocabulary not found."));
        }

        var relationExists = await _context.SentenceVocabularies
            .AnyAsync(sv =>
                sv.SentenceId == sentenceId &&
                sv.VocabularyId == vocabularyId
            );

        if (relationExists)
        {
            // return BadRequest("This vocabulary is already linked to the sentence.");
            return BadRequest(ApiResponse<string>.Fail(400, "This vocabulary is already linked to the sentence."));
        }

        var relation = new SentenceVocabulary
        {
            SentenceId = sentenceId,
            VocabularyId = vocabularyId
        };

        _context.SentenceVocabularies.Add(relation);
        await _context.SaveChangesAsync();

        // return Ok("Vocabulary linked to sentence successfully.");

        return Ok(ApiResponse.NoContent(message: "Vocabulary linked to sentence successfully."));
    }

    // POST: api/Sentence
    [HttpPost]
    public async Task<ActionResult<Sentence>> CreateSentence(Sentence sentence)
    {
        var articleExists = await _context.Articles
            .AnyAsync(a => a.Id == sentence.ArticleId);

        if (!articleExists)
        {
            // return BadRequest("ArticleId does not exist.");
            return BadRequest(ApiResponse<string>.Fail(400, "ArticleId does not exist."));
        }

        _context.Sentences.Add(sentence);
        await _context.SaveChangesAsync();

        // return CreatedAtAction(
        //     nameof(GetSentence),
        //     new { id = sentence.Id },
        //     sentence
        // );

        return Ok(ApiResponse<Sentence>.Success(sentence));
    }

    [HttpPost("{sentenceId}/grammar-points/{grammarPointId}")]
    public async Task<IActionResult> AddGrammarPointToSentence(
    int sentenceId,
    int grammarPointId
)
    {
        var sentenceExists = await _context.Sentences
            .AnyAsync(s => s.Id == sentenceId);

        if (!sentenceExists)
        {
            // return NotFound("Sentence not found.");
            return NotFound(ApiResponse<string>.Fail(404, "Sentence not found."));
        }

        var grammarPointExists = await _context.GrammarPoints
            .AnyAsync(g => g.Id == grammarPointId);

        if (!grammarPointExists)
        {
            // return NotFound("Grammar point not found.");
            return BadRequest(ApiResponse<string>.Fail(404, "Grammar point not found."));
        }

        var relationExists = await _context.SentenceGrammarPoints
            .AnyAsync(sg =>
                sg.SentenceId == sentenceId &&
                sg.GrammarPointId == grammarPointId
            );

        if (relationExists)
        {
            // return BadRequest("This grammar point is already linked to the sentence.");
            return BadRequest(ApiResponse<string>.Fail(400, "This grammar point is already linked to the sentence."));
        }

        var relation = new SentenceGrammarPoint
        {
            SentenceId = sentenceId,
            GrammarPointId = grammarPointId
        };

        _context.SentenceGrammarPoints.Add(relation);
        await _context.SaveChangesAsync();

        // return Ok("Grammar point linked to sentence successfully.");
        return Ok(ApiResponse.NoContent(message: "Grammar point linked to sentence successfully."));
    }

    // PUT: api/Sentence/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSentence(int id, Sentence sentence)
    {
        if (id != sentence.Id)
        {
            // return BadRequest("Route id and sentence id are not the same.");
            return BadRequest(ApiResponse<string>.Fail(400, "Route id and sentence id are not the same."));
        }

        var articleExists = await _context.Articles
            .AnyAsync(a => a.Id == sentence.ArticleId);

        if (!articleExists)
        {
            // return BadRequest("ArticleId does not exist.");
            return BadRequest(ApiResponse<string>.Fail(400, "ArticleId does not exist."));
        }

        var exists = await _context.Sentences
            .AnyAsync(s => s.Id == id);

        if (!exists)
        {
            // return NotFound();
            return NotFound(ApiResponse<string>.Fail(404, "Article not found."));
        }

        _context.Entry(sentence).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse.NoContent());
    }

    // DELETE: api/Sentence/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSentence(int id)
    {
        var sentence = await _context.Sentences.FindAsync(id);

        if (sentence == null)
        {
            // return NotFound();
            return NotFound(ApiResponse<string>.Fail(404, "Sentence not found."));
        }

        _context.Sentences.Remove(sentence);
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse.NoContent());
    }

    // DELETE: api/Sentence/1/vocabularies/{vocabularyId}
    [HttpDelete("{sentenceId}/vocabularies/{vocabularyId}")]
    public async Task<IActionResult> RemoveVocabularyFromSentence(
    int sentenceId,
    int vocabularyId
)
    {
        var relation = await _context.SentenceVocabularies
            .FirstOrDefaultAsync(sv =>
                sv.SentenceId == sentenceId &&
                sv.VocabularyId == vocabularyId
            );

        if (relation == null)
        {
            // return NotFound("Relation not found.");
            return BadRequest(ApiResponse<string>.Fail(404, "Relation not found."));
        }

        _context.SentenceVocabularies.Remove(relation);
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse.NoContent());
    }

    // DELETE: api/Sentence/{sentenceId}/grammar-points/{grammarPointId}
    [HttpDelete("{sentenceId}/grammar-points/{grammarPointId}")]
    public async Task<IActionResult> RemoveGrammarPointFromSentence(
    int sentenceId,
    int grammarPointId
)
    {
        var relation = await _context.SentenceGrammarPoints
            .FirstOrDefaultAsync(sg =>
                sg.SentenceId == sentenceId &&
                sg.GrammarPointId == grammarPointId
            );

        if (relation == null)
        {
            // return NotFound("Relation not found.");
            return BadRequest(ApiResponse<string>.Fail(404, "Relation not found."));
        }

        _context.SentenceGrammarPoints.Remove(relation);
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse.NoContent());
    }
}