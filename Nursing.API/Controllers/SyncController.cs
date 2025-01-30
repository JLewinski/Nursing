// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Nursing.API.Models;
// using Nursing.API.Services;
// using Nursing.Core.Models;
// using System.Security.Claims;

// namespace Nursing.API.Controllers;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// [ApiController]
// [Route("api/[controller]")]
// public class SyncController : ControllerBase
// {
//     private string GetUserId()
//     {
//         var idClaim = User.Claims.First(c => c.Type == ClaimTypes.Sid);
//         return idClaim.Value;
//     }
    
//     [HttpPost("sync", Name = "Sync")]
//     [ProducesResponseType<SyncResult>(200)]
//     public async Task<IActionResult> Sync([FromBody] SyncModel sync)
//     {
//         try
//         {
//             var result = await _syncService.SyncFeedings(sync, GetUserId());
//             return Ok(result);
//         }
//         catch (Exception)
//         {
//             return BadRequest();
//         }
//     }

    

//     [HttpPost("delete", Name = "Delete")]
//     public async Task<IActionResult> Delete([FromBody] Guid[] ids)
//     {
//         var numberUpdated = await _syncService.DeleteFeedings(ids, GetUserId());
//         return Ok(numberUpdated);
//     }
// }