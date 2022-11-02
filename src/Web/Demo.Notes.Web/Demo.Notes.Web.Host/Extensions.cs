using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo.Notes.Web.Host;

public static class Extensions
{
    public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) ?? claimsPrincipal.FindFirst("name");

        if (claim != null)
        {
            var rawValue = claim.Value;
            try
            {
                var items = JsonSerializer.Deserialize<List<string>>(rawValue);
                if (items != null && items.Any())
                {
                    return items.First();
                }
            }
            catch
            {
                // not valid json
            }

            return rawValue;
        }

        return "Unknown";
    }
}