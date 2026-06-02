using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VocabularyController : ControllerBase
{
    private readonly AppDbContext _context;

    public VocabularyController(AppDbContext context)
    {
        _context = context;
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

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vocabulary>> GetVocabulary(int id)
    {
        var vocabulary = await _context.Vocabularies.FindAsync(id);

        if (vocabulary == null)
        {
            return NotFound();
        }

        return Ok(vocabulary);
    }

    [HttpPost]
    public async Task<ActionResult<Vocabulary>> CreateVocabulary(Vocabulary vocabulary)
    {
        _context.Vocabularies.Add(vocabulary);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVocabulary), new { id = vocabulary.Id }, vocabulary);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Vocabulary>> UpdateVocabulary(int id, Vocabulary vocabulary)
    {
        if ( id != vocabulary.Id)
        {
            return BadRequest();
        }

        var exists = await _context.Vocabularies.AnyAsync(s => s.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        _context.Entry(vocabulary).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVocabulary(int id)
    {
        var vocabulary = await _context.Vocabularies.FindAsync(id);

        if (vocabulary == null)
        {
            return NotFound();
        }

        _context.Vocabularies.Remove(vocabulary);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}