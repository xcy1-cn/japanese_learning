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
    /* 
        string? keyword,
        string? level,
        GET /api/GrammarPoint?keyword=ために&level=N4&page=1&pageSize=10    
     */
    public async Task<IActionResult> GetGrammarPoints(
        string? keyword,
        string? level,
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

        var query = _context.GrammarPoints.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(g =>
                g.Title.Contains(keyword) ||
                g.Explanation.Contains(keyword) ||
                g.Structure.Contains(keyword) ||
                g.Example.Contains(keyword)
            );
        }

        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(g => g.Level == level);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(g => g.Id)
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