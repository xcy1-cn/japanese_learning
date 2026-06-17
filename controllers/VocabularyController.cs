using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using JapaneseLearningApi.Responses;
using JapaneseLearningApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VocabularyController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PublicCacheInvalidationService _publicCacheInvalidationService;

    public VocabularyController(
        AppDbContext context,
        PublicCacheInvalidationService publicCacheInvalidationService
    )
    {
        _context = context;
        _publicCacheInvalidationService = publicCacheInvalidationService;
    }

    [HttpGet]
    /* 
    string? word,
    string? level,
    string? partOfSpeech,
    GET /api/Vocabulary?word=学生&level=N5&partOfSpeech=noun&page=1&pageSize=10
     */
    public async Task<IActionResult> GetVocabularies(
        string? word,
        string? level,
        string? partOfSpeech,
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

        var query = _context.Vocabularies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(word))
        {
            query = query.Where(v =>
                v.Word.Contains(word) ||
                v.Reading.Contains(word) ||
                v.Meaning.Contains(word)
            );
        }

        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(v => v.Level == level);
        }

        if (!string.IsNullOrWhiteSpace(partOfSpeech))
        {
            query = query.Where(v => v.PartOfSpeech == partOfSpeech);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(v => v.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PagedResult<Vocabulary>
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

        return Ok(ApiResponse<PagedResult<Vocabulary>>.Success(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vocabulary>> GetVocabulary(int id)
    {
        var vocabulary = await _context.Vocabularies.FindAsync(id);

        if (vocabulary == null)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Vocabulary not found."));
        }

        // return Ok(vocabulary);
        return Ok(ApiResponse<Vocabulary>.Success(vocabulary));
    }

    [HttpPost]
    public async Task<ActionResult<Vocabulary>> CreateVocabulary(Vocabulary vocabulary)
    {
        _context.Vocabularies.Add(vocabulary);
        await _context.SaveChangesAsync();
        _publicCacheInvalidationService.InvalidateArticles();

        // return CreatedAtAction(nameof(GetVocabulary), new { id = vocabulary.Id }, vocabulary);

        return Ok(ApiResponse<Vocabulary>.Success(vocabulary, "Vocabulary created successfully."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Vocabulary>> UpdateVocabulary(int id, Vocabulary vocabulary)
    {
        if ( id != vocabulary.Id)
        {
            return BadRequest(ApiResponse<string>.Fail(400, "This vocabulary id is invalid."));
        }

        var exists = await _context.Vocabularies.AnyAsync(s => s.Id == id);

        if (!exists)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Vocabulary not found."));
        }

        _context.Entry(vocabulary).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        _publicCacheInvalidationService.InvalidateArticles();

        return Ok(ApiResponse.NoContent());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVocabulary(int id)
    {
        var vocabulary = await _context.Vocabularies.FindAsync(id);

        if (vocabulary == null)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Vocabulary not found."));
        }

        _context.Vocabularies.Remove(vocabulary);
        await _context.SaveChangesAsync();
        _publicCacheInvalidationService.InvalidateArticles();

        return Ok(ApiResponse.NoContent());
    }
}