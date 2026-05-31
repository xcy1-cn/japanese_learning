using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
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
    public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
    {
        var questions = await _context.Questions.ToListAsync();

        return Ok(questions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
        {
            return NotFound();
        }

        return Ok(question);
    }

    [HttpPost]
    public async Task<ActionResult<Question>> CreateVocabulary(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Question>> UpdateVocabulary(int id, Question question)
    {
        if (id != question.Id)
        {
            return BadRequest();
        }

        var exists = await _context.Questions.AnyAsync(s => s.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        _context.Entry(question).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question == null)
        {
            return NotFound();
        }

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}