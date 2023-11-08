using Microsoft.AspNetCore.Mvc;

using TokensApi.Utils;

namespace TokensApi;

[ApiController]
[Route("/api/v1/token")]
public class TokenController : ControllerBase
{
    private readonly IJwtWriter _writer;

    public TokenController(IJwtWriter writer)
    {
        _writer = writer;
    }

    [HttpPost]
    [Route("issue")]
    public IActionResult Issue([FromBody] AuthRequest req)
    {
        var resp = _writer.IssueToken(req);

        return resp is null
            ? Unauthorized()
            : Ok(resp);
    }
}
