using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeHxApp.Server.Data;

namespace ResumeHxApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ResumesController> _logger;

    public ResumesController(ApplicationDbContext context, ILogger<ResumesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Resume>>> GetResumes()
    {
        return await _context.Resumes
            .Include(r => r.JobHistories)
            .ThenInclude(j => j.Responsibilities)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Resume>> GetResume(int id)
    {
        var resume = await _context.Resumes
            .Include(r => r.JobHistories)
            .ThenInclude(j => j.Responsibilities)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (resume == null)
        {
            return NotFound();
        }

        return resume;
    }

    [HttpPost]
    public async Task<ActionResult<Resume>> CreateResume(Resume resume)
    {
        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetResume), new { id = resume.Id }, resume);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResume(int id, Resume resume)
    {
        if (id != resume.Id)
        {
            return BadRequest();
        }

        resume.UpdatedAt = DateTime.UtcNow;
        _context.Entry(resume).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ResumeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResume(int id)
    {
        var resume = await _context.Resumes.FindAsync(id);
        if (resume == null)
        {
            return NotFound();
        }

        _context.Resumes.Remove(resume);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> ResumeExists(int id)
    {
        return await _context.Resumes.AnyAsync(e => e.Id == id);
    }
}
