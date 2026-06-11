using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using JapaneseLearningApi.Responses;
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

        // return Ok(new
        // {
        //     total,
        //     page,
        //     pageSize,
        //     items
        // });

        var data = new PagedResult<GrammarPoint>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };

        // return Ok(ApiResponse<GrammarPointResponse>.Success(data, "success"));
        return Ok(ApiResponse<PagedResult<GrammarPoint>>.Success(data));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GrammarPoint>> GetGrammarPoint(int id)
    {
        var grammarPoint = await _context.GrammarPoints.FindAsync(id);

        if (grammarPoint == null)
        {
            return BadRequest(ApiResponse<string>.Fail(404, "Invalid grammarPoint."));
        }

        // return Ok(grammarPoint);
        return Ok(ApiResponse<GrammarPoint>.Success(grammarPoint, "Searching grammarPoint successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<GrammarPoint>> CreateGrammarPoint(GrammarPoint grammarPoint)
    {
        _context.GrammarPoints.Add(grammarPoint);
        await _context.SaveChangesAsync();

        // return CreatedAtAction(nameof(GetGrammarPoint), new { id = grammarPoint.Id }, grammarPoint);

        return Ok(ApiResponse<GrammarPoint>.Success(grammarPoint));
    }

    // POST: {sentenceId}/grammar-points/{grammarPointId}
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
            return BadRequest(ApiResponse<string>.Fail(404, "Sentence not found."));
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

        return Ok(ApiResponse<SentenceGrammarPoint>.Success(relation, "Grammar point linked to sentence successfully."));

    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GrammarPoint>> UpdateGrammarPoint(int id, GrammarPoint grammarPoint)
    {
        if (id != grammarPoint.Id)
        {
            // return BadRequest();
            return BadRequest(ApiResponse<string>.Fail(400, "This grammar point id is invalid."));
        }

        var exists = await _context.GrammarPoints.AnyAsync(s => s.Id == id);

        if (!exists)
        {
            return BadRequest(ApiResponse<string>.Fail(404, "Grammar point not found."));
        }

        _context.Entry(grammarPoint).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse<GrammarPoint>.Success(grammarPoint, "GrammarPoint changes successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrammarPoint(int id)
    {
        var grammarPoint = await _context.GrammarPoints.FindAsync(id);

        if (grammarPoint == null)
        {
            return BadRequest(ApiResponse<string>.Fail(404, "Grammar point not found."));
        }

        _context.GrammarPoints.Remove(grammarPoint);
        await _context.SaveChangesAsync();


        // return NoContent();
        return Ok(ApiResponse<GrammarPoint>.Success(grammarPoint, "GrammarPoint removes successfully."));
    }
}