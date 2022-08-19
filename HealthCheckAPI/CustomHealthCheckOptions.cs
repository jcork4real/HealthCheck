/*
 * we override the standard class—which outputs the one-word output 
 * we want to change—with our own custom class so that we can change 
 * its ResponseWriter property, in order to make it output whatever we want.
 * More specifically, we want to output a custom JSON-structured message
 */
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;
namespace HealthCheckAPI
{
    public class CustomHealthCheckOptions : HealthCheckOptions
    {
        public CustomHealthCheckOptions() : base()
        {
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            ResponseWriter = async (c, r) =>
            {
                c.Response.ContentType =
                 MediaTypeNames.Application.Json;
                c.Response.StatusCode = StatusCodes.Status200OK;
                var result = JsonSerializer.Serialize(new
                {
                    checks = r.Entries.Select(e => new
                    {
                        name = e.Key,
                        responseTime =
                             e.Value.Duration.TotalMilliseconds,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    }),
                    totalStatus = r.Status,
                    totalResponseTime =
                       r.TotalDuration.TotalMilliseconds,
                }, jsonSerializerOptions);
                await c.Response.WriteAsync(result);
            };
        }
    }
}