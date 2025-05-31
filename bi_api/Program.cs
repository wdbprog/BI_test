using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

//set up CORS for frontend
var frontendUrl = builder.Configuration.GetValue<string>("frontendUrl");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

var biApiUrl = builder.Configuration.GetValue<string>("BiApiUrl");

app.MapGet("/calendar-events", async (
    int? take,
    string? language,
    string? campus,
    string? audience,
    HttpClient client,
    IMemoryCache cache) =>
{
    var cacheKey = $"calendar-events-{take}-{language}-{campus}-{audience}";

    //try to get cached event first
    if (!cache.TryGetValue(cacheKey, out JsonNode? events))
    {
        var uriBuilder = new UriBuilder(biApiUrl);
        var query = new List<string>();

        //build filter query parameters
        if (take is not null && take != 5)
            query.Add($"Take={take}");
        if (!string.IsNullOrWhiteSpace(language) && !string.Equals(language, "all", StringComparison.OrdinalIgnoreCase))
            query.Add($"Language={language}");
        if (!string.IsNullOrWhiteSpace(campus))
            query.Add($"Campus={campus}");
        if (!string.IsNullOrWhiteSpace(audience))
            query.Add($"Audience={audience}");

        uriBuilder.Query = string.Join("&", query);

        var response = await client.GetAsync(uriBuilder.Uri);
        response.EnsureSuccessStatusCode();

        //possible improvement, could deserialize to a strongly typed object, but would require more changes if API output format changes
        var jsonString = await response.Content.ReadAsStringAsync();
        events = JsonNode.Parse(jsonString);

        //Cache results for 5min
        cache.Set(cacheKey, events, TimeSpan.FromMinutes(5));
    }

    return Results.Ok(events);

})
.WithName("GetCalendarEvents")
.WithOpenApi();

app.Run();

public partial class Program { }
