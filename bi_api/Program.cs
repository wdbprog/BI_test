
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

const string biApiUrl = "https://bi.no/api/calendar-events"; //maybe move to appsettings.json?

app.MapGet("/calendar-events", async (
    int? take,
    string? language,
    string? campus,
    string? audience,
    HttpClient client) =>
{
    var uriBuilder = new UriBuilder(biApiUrl);
    //var query = new List<string>
    //{
    //    $"Take={Take ?? 5}",
    //    $"Language={Language ?? "all"}"
    //};
    //if (!string.IsNullOrWhiteSpace(Campus))
    //    query.Add($"Campus={Campus}");
    //if (!string.IsNullOrWhiteSpace(Audience))
    //    query.Add($"Audience={Audience}");

    //uriBuilder.Query = string.Join("&", query);

    var response = await client.GetAsync(biApiUrl);
    response.EnsureSuccessStatusCode();

    var events = await response.Content.ReadAsStringAsync();
    return Results.Ok(events);

})
.WithName("GetCalendarEvents")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
