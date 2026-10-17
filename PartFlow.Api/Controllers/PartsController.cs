using Microsoft.AspNetCore.Mvc;
using PartFlow.Api.Models;
using PartFlow.Api.Services;

namespace PartFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly InMemoryPartStore _store;

    public PartsController(InMemoryPartStore store)
    {
        _store = store;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<Part>> GetAll() => Ok(_store.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Part> GetById(int id)
    {
        var part = _store.GetById(id);
        return part is null ? NotFound() : Ok(part);
    }

    [HttpPost]
    public ActionResult<Part> Create(PartRequest request)
    {
        var part = _store.Add(request);
        return CreatedAtAction(nameof(GetById), new { id = part.Id }, part);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Part> Update(int id, PartRequest request)
    {
        var part = _store.Update(id, request);
        return part is null ? NotFound() : Ok(part);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) =>
        _store.Delete(id) ? NoContent() : NotFound();
}
