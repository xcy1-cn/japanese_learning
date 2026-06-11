using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using JapaneseLearningApi.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuestionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    /* 
    GET /api/Question?type=vocabulary_choice&keyword=学生&page=1&pageSize=10
     */
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

        // if (!string.IsNullOrWhiteSpace(level))
        // {
        //     query = query.Where(q => q.Level == level);
        // }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(q =>
                q.Stem.Contains(keyword) ||
                q.OptionA.Contains(keyword) ||
                q.OptionB.Contains(keyword) ||
                q.OptionC.Contains(keyword) ||
                q.OptionD.Contains(keyword) ||
                q.Explanation.Contains(keyword)
            );
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(q => q.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = new PagedResult<Question>
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

        return Ok(ApiResponse<PagedResult<Question>>.Success(data));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Question not found."));
        }

        // return Ok(question);

        return Ok(ApiResponse<Question>.Success(question, "Searching question successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<Question>> CreateVocabulary(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        // return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);

        return Ok(ApiResponse<Question>.Success(question));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Question>> UpdateVocabulary(int id, Question question)
    {
        if (id != question.Id)
        {
            // return BadRequest();
            return BadRequest(ApiResponse<string>.Fail(404, "Sentence not found."));
        }

        var exists = await _context.Questions.AnyAsync(s => s.Id == id);

        if (!exists)
        {
            // return NotFound();
            return BadRequest(ApiResponse<string>.Fail(404, "Question not found."));
        }

        _context.Entry(question).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse.NoContent());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
        {
            // return NotFound();
            return BadRequest(ApiResponse<string>.Fail(404, "Sentence not found."));
        }

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(ApiResponse.NoContent());
    }

}