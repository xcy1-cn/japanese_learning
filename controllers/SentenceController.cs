using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
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

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    // GET: api/Sentence/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Sentence>> GetSentence(int id)
    {
        var sentence = await _context.Sentences
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sentence == null)
        {
            return NotFound();
        }

        return Ok(sentence);
    }

    // POST: api/Sentence
    [HttpPost]
    public async Task<ActionResult<Sentence>> CreateSentence(Sentence sentence)
    {
        var articleExists = await _context.Articles
            .AnyAsync(a => a.Id == sentence.ArticleId);

        if (!articleExists)
        {
            return BadRequest("ArticleId does not exist.");
        }

        _context.Sentences.Add(sentence);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSentence),
            new { id = sentence.Id },
            sentence
        );
    }

    // PUT: api/Sentence/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSentence(int id, Sentence sentence)
    {
        if (id != sentence.Id)
        {
            return BadRequest("Route id and sentence id are not the same.");
        }

        var articleExists = await _context.Articles
            .AnyAsync(a => a.Id == sentence.ArticleId);

        if (!articleExists)
        {
            return BadRequest("ArticleId does not exist.");
        }

        var exists = await _context.Sentences
            .AnyAsync(s => s.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        _context.Entry(sentence).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Sentence/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSentence(int id)
    {
        var sentence = await _context.Sentences.FindAsync(id);

        if (sentence == null)
        {
            return NotFound();
        }

        _context.Sentences.Remove(sentence);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}