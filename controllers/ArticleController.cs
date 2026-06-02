using JapaneseLearningApi.Data;
using JapaneseLearningApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Controllers;

[Authorize]
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
    /* string? keyword,
    string? level,
    string? category,
    http://localhost:5251/api/Article?keyword=?&level=N5&category=reading&page=1&pageSize=10
     */
    public async Task<IActionResult> GetArticles(
    string? keyword,
    string? level,
    string? category,
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

        // AsQueryable()创建一个可以继续拼接条件的查询对象,AsQueryable 本身还不会真正查询数据库。
        // 只有 CountAsync / ToListAsync 时才会真正执行 SQL。
        var query = _context.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            //搜索标题或正文中包含 keyword 的文章
            query = query.Where(a =>
                a.Title.Contains(keyword) ||
                a.Content.Contains(keyword)
            );
        }

        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(a => a.Level == level);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }

        //CountAsync()统计筛选条件下总共有多少条数据
        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.Id)
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