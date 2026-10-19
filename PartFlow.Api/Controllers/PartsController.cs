
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PartFlow.Api.Data;
using PartFlow.Api.Models;

namespace PartFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private const string PartNumberUniqueIndex = "IX_Parts_PartNumber";
    private readonly PartFlowDbContext _db;

    public PartsController(PartFlowDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Part>>> GetAll()
    {
        var parts = await _db.Parts
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(parts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Part>> GetById(int id)
    {
        var part = await _db.Parts.FindAsync(id);

        return part is null ? NotFound() : Ok(part);
    }

    [HttpPost]
    public async Task<ActionResult<Part>> Create(PartRequest request)
    {
        var part = new Part
        {
            PartNumber = request.PartNumber.Trim(),
            Name = request.Name.Trim(),
            Description = request.Description,
            Status = request.Status
        };

        _db.Parts.Add(part);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
            when (IsPartNumberConflict(exception))
        {
            return PartNumberConflict();
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = part.Id },
            part);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Part>> Update(
        int id, PartRequest request)
    {
        var part = await _db.Parts.FindAsync(id);

        if (part is null)
            return NotFound();

        if (!PartLifecycle.CanTransition(part.Status, request.Status))
        {
            return BadRequest(new
            {
                error = $"Invalid lifecycle transition from {part.Status} to {request.Status}."
            });
        }

        part.PartNumber = request.PartNumber.Trim();
        part.Name = request.Name.Trim();
        part.Description = request.Description;
        part.Status = request.Status;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
            when (IsPartNumberConflict(exception))
        {
            return PartNumberConflict();
        }

        return Ok(part);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var part = await _db.Parts.FindAsync(id);

        if (part is null)
            return NotFound();

        _db.Parts.Remove(part);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static bool IsPartNumberConflict(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: PartNumberUniqueIndex
        };
    }

    private ConflictObjectResult PartNumberConflict()
    {
        return Conflict(new
        {
            error = "A part with this PartNumber already exists."
        });
    }
}
