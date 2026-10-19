using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PartFlow.Api.Controllers;
using PartFlow.Api.Data;
using PartFlow.Api.Models;

namespace PartFlow.Tests;

public class PartsControllerTests
{
    private const string PartNumberUniqueIndex =
        "IX_Parts_PartNumber";

    [Fact]
    public async Task Create_ReturnsConflict_ForDuplicatePartNumber()
    {
        await using var db = CreateThrowingDbContext(
            PostgresErrorCodes.UniqueViolation,
            PartNumberUniqueIndex);
        var controller = new PartsController(db);

        var result = await controller.Create(CreateRequest("PF-1000"));

        AssertConflict(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsConflict_ForDuplicatePartNumber()
    {
        await using var db = CreateThrowingDbContext(
            PostgresErrorCodes.UniqueViolation,
            PartNumberUniqueIndex);
        db.Parts.Attach(new Part
        {
            Id = 1,
            PartNumber = "PF-1000",
            Name = "Existing part",
            Status = PartStatus.Draft
        });
        var controller = new PartsController(db);

        var result = await controller.Update(
            1,
            CreateRequest("PF-2000"));

        AssertConflict(result.Result);
    }

    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "IX_Other")]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation,
        PartNumberUniqueIndex)]
    public async Task Create_DoesNotCatch_UnrelatedDatabaseErrors(
        string sqlState,
        string constraintName)
    {
        await using var db = CreateThrowingDbContext(
            sqlState,
            constraintName);
        var controller = new PartsController(db);

        await Assert.ThrowsAsync<DbUpdateException>(
            () => controller.Create(CreateRequest("PF-1000")));
    }

    private static PartRequest CreateRequest(string partNumber)
    {
        return new PartRequest
        {
            PartNumber = partNumber,
            Name = "Test part",
            Status = PartStatus.Draft
        };
    }

    private static ThrowingPartFlowDbContext CreateThrowingDbContext(
        string sqlState,
        string constraintName)
    {
        var postgresException = new PostgresException(
            messageText: "Database error",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: sqlState,
            detail: null,
            hint: null,
            position: 0,
            internalPosition: 0,
            internalQuery: null,
            where: null,
            schemaName: "public",
            tableName: "Parts",
            columnName: "PartNumber",
            dataTypeName: null,
            constraintName: constraintName,
            file: null,
            line: null,
            routine: null);

        return new ThrowingPartFlowDbContext(
            new DbUpdateException("Save failed", postgresException));
    }

    private static void AssertConflict(IActionResult? result)
    {
        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);

        var error = conflict.Value?
            .GetType()
            .GetProperty("error")?
            .GetValue(conflict.Value);

        Assert.Equal(
            "A part with this PartNumber already exists.",
            error);
    }

    private sealed class ThrowingPartFlowDbContext(
        DbUpdateException exception)
        : PartFlowDbContext(
            new DbContextOptionsBuilder<PartFlowDbContext>()
                .UseNpgsql("Host=unused")
                .Options)
    {
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromException<int>(exception);
        }
    }
}
