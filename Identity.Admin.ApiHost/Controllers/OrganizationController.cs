using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Upo.Identity.Admin.ApiHost.Controllers
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IMutableOrganizationService _service;
        private readonly IOrganizationProvider _provider;
        public OrganizationController(
            IMutableOrganizationService service,
            IOrganizationProvider provider)
        {
            this._service = service;
            this._provider = provider;
        }

        #region CRUD
        [HttpGet("{organizationId}/users")]
        [ProducesResponseType(typeof(PagingResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Users([FromRoute, Required]Guid organizationId, [FromQuery]PagingContext context)
        {
            return Ok(await this._provider.PagingUsersAsync(organizationId, context).ConfigureAwait(false));
        }

        [HttpGet("roots")]
        [ProducesResponseType(typeof(IEnumerable<IdentityOrganization>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Roots()
        {
            return Ok(await this._provider.GetChildrenAsync(null).ConfigureAwait(false));
        }

        [HttpGet("{parentId}/children")]
        [ProducesResponseType(typeof(IEnumerable<IdentityOrganization>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Children([FromRoute, Required]Guid parentId)
        {
            return Ok(await this._provider.GetChildrenAsync(parentId).ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IdentityOrganization), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            return Ok(await this._provider.FindAsync(id).ConfigureAwait(false));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdentityOrganization), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody, Required] IdentityOrganization organization)
        {
            return Ok(await this._service.CreateAsync(organization).ConfigureAwait(false));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(IdentityOrganization), StatusCodes.Status200OK)]
        public async Task<IActionResult> Patch([FromRoute, Required]Guid id, [FromBody, Required] IDictionary<string, object> updateProperties)
        {
            return Ok(await this._service.UpdateAsync(id, updateProperties).ConfigureAwait(false));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IdentityOrganization), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute, Required]Guid id)
        {
            return Ok(await this._service.DeleteAsync(id).ConfigureAwait(false));
        }
        #endregion

        #region Extensions

        [HttpPost("{organizationId}/user")]
        [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUser([FromRoute, Required] Guid organizationId, [FromBody, Required]IdentityUser user)
        {
            return Ok(await this._service.AddUserAsync(organizationId, user).ConfigureAwait(false));
        }

        [HttpPost("{organizationId}/user/{userId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserSecondary([FromRoute, Required] Guid organizationId, [FromRoute, Required]Guid userId)
        {
            await this._service.AddUserAsync(organizationId, userId).ConfigureAwait(false);

            return Ok();
        }

        [HttpDelete("{organizationId}/user/{userId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveUser([FromRoute, Required] Guid organizationId, [FromRoute, Required]Guid userId)
        {
            await this._service.RemoveUserAsync(organizationId, userId).ConfigureAwait(false);

            return Ok();
        }

        [HttpDelete("{organizationId}/users")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveUsers([FromRoute, Required] Guid organizationId, [FromBody, Required]Guid[] userIds)
        {
            await this._service.RemoveUsersAsync(organizationId, userIds).ConfigureAwait(false);

            return Ok();
        }

        #endregion
    }
}
