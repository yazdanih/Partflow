namespace PartFlow.Api.Models;

public class PartRequest
{
    public required string PartNumber { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public PartStatus Status { get; set; } = PartStatus.Draft;
}
