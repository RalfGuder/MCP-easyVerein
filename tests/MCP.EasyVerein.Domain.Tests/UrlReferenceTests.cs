using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Tests;

public class UrlReferenceTests
{
    [Theory]
    [InlineData("https://easyverein.com/api/v1.7/contact-details/345175845", 345175845L)]
    [InlineData("https://easyverein.com/api/v1.7/booking/234717573/", 234717573L)]
    [InlineData("https://easyverein.com/api/v1.4/invoice/1", 1L)]
    public void ExtractId_ReturnsTrailingNumber(string url, long expected)
    {
        Assert.Equal(expected, UrlReference.ExtractId(url));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("https://easyverein.com/api/v1.7/contact-details/")]
    public void ExtractId_ReturnsNullForInvalidInput(string? url)
    {
        Assert.Null(UrlReference.ExtractId(url));
    }
}
