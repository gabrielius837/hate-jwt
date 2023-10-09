using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Issue([FromBody] AuthenticationRequestDto dto)
    {
        var req = dto.TryBuild();

        if (req is null)
        {
            return BadRequest();
        }

        var resp = _writer.IssueToken(req);

        return resp is null
            ? Unauthorized()
            : Ok(resp);
    }
}
