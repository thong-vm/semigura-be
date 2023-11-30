using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using semigura.DBContext.Entities;
using semigura.Hubs;
using semigura.Models;
using System.IdentityModel.Tokens.Jwt;

namespace semigura.Controllers
{

    [Route("/api/[controller]")]
    [Authorize]
    public class CommandsController : ControllerBase
    {
        private readonly DBEntities _db;
        private readonly IChatHubRepository _chatHub;
        public CommandsController(DBEntities dBEntities,
            IChatHubRepository chatHub)
        {
            _db = dBEntities;
            _chatHub = chatHub;
        }

        // POST: api/commands
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Command command)
        {
            try
            {
                switch (command.Event)
                {
                    case "OnSensorUpdated":
                        await _chatHub.NotifyOnSensorUpdated();
                        return Ok("success!");
                    case "TestConnection":
                        var result = new string[] { "", "" };
                        try
                        {
                            result[1] = _db.Database.CanConnect() ? "OK" : "NG";
                        }
                        catch (Exception ex)
                        {
                            result[1] = "NG: " + ex.Message;
                        }

                        return Ok(new { Result = string.Join(", ", result) });
                    case "DecodeToken":
                        var token = command.Result;
                        var handler = new JwtSecurityTokenHandler();
                        var jwtSecurityToken = handler.ReadJwtToken(token);
                        return Ok(jwtSecurityToken.Payload);
                }
                return BadRequest("Invalid command");
            }
            catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }
    }

}
