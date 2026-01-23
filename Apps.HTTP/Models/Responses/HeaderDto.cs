namespace Apps.HTTP.Models.Responses;

public class HeaderDto
{
    public HeaderDto()
    {
        Name = string.Empty;
        Value = null;
    }
    
    public HeaderDto(string name, string? value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }

    public string? Value { get; set; }
}