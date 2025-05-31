using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;

namespace bi_api_test
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UnitTest1(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace HttpClient with a mock handler
                    services.AddHttpClient("TestClient", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost/");
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler());
                });
            });
        }

        [Theory]
        [InlineData(null, null, null, null, 5)] // No filters, expect all
        [InlineData(2, null, null, null, 2)] // Take 2
        [InlineData(null, "en", null, null, 2)] // Language filter
        [InlineData(null, null, "Oslo", null, 1)] // Campus filter
        [InlineData(null, null, null, "students", 2)] // Audience filter
        public async Task CalendarEventsEndpointTest(
            int? take, string language, string campus, string audience, int expectedCount)
        {
            // Get the HttpClientFactory from services and use it to create the named client
            var httpClientFactory = _factory.Services.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("TestClient");

            var url = "/calendar-events?";
            if (take != null) url += $"take={take}&";
            if (language != null) url += $"language={language}&";
            if (campus != null) url += $"campus={campus}&";
            if (audience != null) url += $"audience={audience}&";

            var response = await client.GetAsync(url.TrimEnd('&', '?'));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(content);

            Assert.NotNull(json);
            Assert.True(json is JsonArray || json?["events"] is JsonArray);

            var events = json as JsonArray ?? json?["events"]?.AsArray();
            Assert.NotNull(events);
            Assert.Equal(expectedCount, events.Count);
        }
    }
}