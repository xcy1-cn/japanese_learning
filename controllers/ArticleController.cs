using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly AppDbContext _context;

    public ArticleController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
    {
        return await _context.Articles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        return article;
    }

    [HttpPost]
    public async Task<ActionResult<Article>> CreateArticle(Article article)
    {
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticle(int id, Article article)
    {
        if (id != article.Id)
        {
            return BadRequest();
        }

        _context.Entry(article).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}