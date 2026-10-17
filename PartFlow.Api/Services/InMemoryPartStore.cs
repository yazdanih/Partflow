using System.Collections.Concurrent;
using PartFlow.Api.Models;

namespace PartFlow.Api.Services;

public class InMemoryPartStore
{
    private readonly ConcurrentDictionary<int, Part> _parts = new();
    private int _nextId;

    public InMemoryPartStore()
    {
        Add(new PartRequest
        {
            PartNumber = "PF-1001",
            Name = "Hydraulic Fitting",
            Description = "Sample part so GET /api/parts has data immediately.",
            Status = PartStatus.Active
        });
    }

    public IReadOnlyList<Part> GetAll() =>
        _parts.Values.OrderBy(part => part.Id).ToList();

    public Part? GetById(int id) =>
        _parts.TryGetValue(id, out var part) ? part : null;

    public Part Add(PartRequest request)
    {
        var id = Interlocked.Increment(ref _nextId);
        var part = ToPart(id, request);
        _parts[id] = part;
        return part;
    }

    public Part? Update(int id, PartRequest request)
    {
        if (!_parts.ContainsKey(id))
        {
            return null;
        }

        var part = ToPart(id, request);
        _parts[id] = part;
        return part;
    }

    public bool Delete(int id) => _parts.TryRemove(id, out _);

    private static Part ToPart(int id, PartRequest request) => new()
    {
        Id = id,
        PartNumber = request.PartNumber.Trim(),
        Name = request.Name.Trim(),
        Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
        Status = request.Status
    };
}
