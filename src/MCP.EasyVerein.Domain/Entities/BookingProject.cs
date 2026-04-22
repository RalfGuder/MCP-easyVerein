using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a booking project (Buchungsprojekt) from the easyVerein API.
/// </summary>
public class BookingProject : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the booking project name. Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color value (max 7 characters). Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.Color)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label (max 4 characters). Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the project budget. Maps to API field '<c>budget</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.Budget)]
    public decimal? Budget { get; set; }

    /// <summary>Gets or sets the completion flag. Maps to API field '<c>completed</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.Completed)]
    public bool? Completed { get; set; }

    /// <summary>Gets or sets the project cost centre. Maps to API field '<c>projectCostCentre</c>'.</summary>
    [JsonPropertyName(BookingProjectFields.ProjectCostCentre)]
    public string? ProjectCostCentre { get; set; }
}
