using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Requests;

public class AddressDto
{
    [JsonPropertyName("zipCode")]
    public string ZipCode { get; set; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("complement")]
    public string? Complement { get; set; }

    [JsonPropertyName("neighborhood")]
    public string Neighborhood { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
}
