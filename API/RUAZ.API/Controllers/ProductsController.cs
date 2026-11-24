using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RUAZ.API.Authorization;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Services;
using RUAZ.Domain.Enums;

namespace RUAZ.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AuthPolicies.User)]
    public class ProductsController(
        ProductService productService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponse>>> GetAll([FromQuery] ProductFilterRequest filter)
        {
            var result = await productService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetById(Guid id)
        {
            var result = await productService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
        {
            var userId = GetCurrentUserId();
            var created = await productService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request)
        {
            var userId = GetCurrentUserId();
            var updated = await productService.UpdateAsync(id, request, userId);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserTypeActivity();
            await productService.DeleteAsync(id, userId, userRole);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var rawId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(rawId, out var userId))
            {
                throw new UnauthorizedException("Identificador do usuário ausente ou inválido no token.");
            }

            return userId;
        }

        private TypeActivity GetCurrentUserTypeActivity()
        {
            var rawRole = User.FindFirstValue("type_activity")
                ?? User.FindFirstValue(ClaimTypes.Role);

            if (!Enum.TryParse<TypeActivity>(rawRole, out var typeActivity))
            {
                throw new UnauthorizedException("Tipo de atividade ausente ou inválido no token.");
            }

            return typeActivity;
        }
    }
}
