using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrammarPointController : ControllerBase
{
    private readonly AppDbContext _context;

    public GrammarPointController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GrammarPoint>>> GetGrammarPoints()
    {
        var grammarPoints = await _context.GrammarPoints.ToListAsync();

        return Ok(grammarPoints);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GrammarPoint>> GetGrammarPoint(int id)
    {
        var grammarPoint = await _context.GrammarPoints.FindAsync(id);

        if (grammarPoint == null)
        {
            return NotFound();
        }

        return Ok(grammarPoint);
    }

    [HttpPost]
    public async Task<ActionResult<GrammarPoint>> CreateGrammarPoint(GrammarPoint grammarPoint)
    {
        _context.GrammarPoints.Add(grammarPoint);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGrammarPoint), new { id = grammarPoint.Id }, grammarPoint);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GrammarPoint>> UpdateGrammarPoint(int id, GrammarPoint grammarPoint)
    {
        if (id != grammarPoint.Id)
        {
            return BadRequest();
        }

        var exists = await _context.GrammarPoints.AnyAsync(s => s.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        _context.Entry(grammarPoint).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrammarPoint(int id)
    {
        var grammarPoint = await _context.GrammarPoints.FindAsync(id);

        if (grammarPoint == null)
        {
            return NotFound();
        }

        _context.GrammarPoints.Remove(grammarPoint);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}