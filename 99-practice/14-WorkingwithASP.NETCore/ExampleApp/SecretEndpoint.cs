using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp;

public class SecretEndpoint {

    public static async Task Endpoint(HttpContext context) {
        await context.Response.WriteAsync("This is the secret message");
    }
}

