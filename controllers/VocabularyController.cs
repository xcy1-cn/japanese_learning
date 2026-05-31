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
    public async Task<ActionResult<IEnumerable<Vocabulary>>> GetVocabularies()
    {
        var vocabularies = await _context.Vocabularies.ToListAsync();

        return Ok(vocabularies);
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