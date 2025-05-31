using System.Net;
using System.Text;

namespace bi_api_test
{
    //Full mock of the BI API endpoint, might be overkill but closest to integration test without hitting the real API
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var events = new[]
            {
                new { title = "Event 1", language = "en", campus = "Bergen", audience = "students" },
                new { title = "Event 2", language = "en", campus = "Oslo", audience = "staff" },
                new { title = "Event 3", language = "no", campus = "Trondheim", audience = "students" },
                new { title = "Event 4", language = "no", campus = "Bergen", audience = "alumni" },
                new { title = "Event 5", language = "sv", campus = "Stockholm", audience = "guests" }
            };

            var query = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query);

            var filtered = events.AsEnumerable();

            if (!string.IsNullOrEmpty(query["language"]))
                filtered = filtered.Where(e => e.language == query["language"]);
            if (!string.IsNullOrEmpty(query["campus"]))
                filtered = filtered.Where(e => e.campus == query["campus"]);
            if (!string.IsNullOrEmpty(query["audience"]))
                filtered = filtered.Where(e => e.audience == query["audience"]);
            if (int.TryParse(query["take"], out int take))
                filtered = filtered.Take(take);

            var json = System.Text.Json.JsonSerializer.Serialize(filtered);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}