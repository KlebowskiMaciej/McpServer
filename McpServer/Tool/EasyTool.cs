
using ModelContextProtocol.Server;

[McpServerToolType]
public static class EasyTool
{
    [McpServerTool]
    public static async Task<string> EasyToolAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            var response = await httpClient.GetStringAsync("http://localhost:8082/swagger/v1/swagger.json");
            var swaggerDoc = System.Text.Json.JsonDocument.Parse(response);
            
            var endpoints = new List<string>();
            
            if (swaggerDoc.RootElement.TryGetProperty("paths", out var pathsElement))
            {
                foreach (var path in pathsElement.EnumerateObject())
                {
                    foreach (var method in path.Value.EnumerateObject())
                    {
                        var endpointName = $"{method.Name.ToUpper()} {path.Name}";
                        endpoints.Add(endpointName);
                    }
                }
            }
            
            return endpoints.Count > 0 
                ? $"Found {endpoints.Count} API endpoints:\n" + string.Join("\n", endpoints)
                : "No API endpoints found in Swagger documentation.";
        }
        catch (HttpRequestException ex)
        {
            return $"Error fetching Swagger documentation: {ex.Message}";
        }
        catch (System.Text.Json.JsonException ex)
        {
            return $"Error parsing Swagger JSON: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Unexpected error: {ex.Message}";
        }
    }
}