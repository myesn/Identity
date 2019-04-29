using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Upo.Identity.Admin.ApiHost.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMutableUserService _service;
        private readonly IUserProvider _provider;
        private const long MaxAvatarFileSize = 1024 * 50;
        private readonly string _defaultAvatarFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "default_avatar.jpeg");

        public UserController(
            IMutableUserService service,
            IUserProvider provider)
        {
            this._service = service;
            this._provider = provider;
        }

        #region CRUD

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute, Required]Guid id)
        {
            return Ok(await this._provider.FindAsync(id).ConfigureAwait(false));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Patch([FromRoute, Required]Guid id, [FromBody, Required] IDictionary<string, object> updateProperties)
        {
            return Ok(await this._service.UpdateAsync(id, updateProperties).ConfigureAwait(false));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute, Required]Guid id)
        {
            return Ok(await this._service.DeleteAsync(id).ConfigureAwait(false));
        }

        #endregion

        #region Extensions

        [HttpGet("{id}/mainorganization")]
        [ProducesResponseType(typeof(IdentityOrganization), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMainOrganization([FromRoute, Required]Guid id)
        {
            return Ok(await this._provider.GetMainOrganizationAsync(id).ConfigureAwait(false));
        }

        [HttpPatch("{userId}/mainorganization/{newMainOrganizationId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeMainOrganization([FromRoute, Required]Guid userId, [FromRoute, Required] Guid newMainOrganizationId)
        {
            await this._service.ChangeMainOrganizationAsync(userId, newMainOrganizationId).ConfigureAwait(false);

            return Ok();
        }

        [HttpPatch("{id}/avatar")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAvatar([FromRoute, Required]Guid id, [Required]IFormFile file)
        {
            if (file.Length > MaxAvatarFileSize)
                throw new Exception("只能上传 50 KB 以内的文件");

            using (var read = file.OpenReadStream())
            using (var memory = new MemoryStream())
            {
                await read.CopyToAsync(memory).ConfigureAwait(false);
                await _service.UpdateAsync(id, new Dictionary<string, object>
                {
                    { nameof(IdentityUser.Avatar), memory.ToArray()}
                }).ConfigureAwait(false);
            }
            return Ok();
        }


        [HttpGet("{id}/avatar")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvatar([FromRoute, Required]Guid id)
        {
            var bytes = await _provider.GetAvatarAsync(id).ConfigureAwait(false);
            if (bytes == null)
            {
                if (!System.IO.File.Exists(_defaultAvatarFilePath))
                    throw new Exception("找不到默认头像");

                bytes = await System.IO.File.ReadAllBytesAsync(_defaultAvatarFilePath).ConfigureAwait(false);
            }

            return File(bytes, "image/png");
        }

        #endregion
    }
}
