using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropertyTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public PropertyTypesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/PropertyTypes
        [HttpGet]
        public IEnumerable<PropertyType> GetPropertyTypes()
        {
            return _context.PropertyTypes.OrderBy(p => p.Name);
        }

        // GET: api/PropertyTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PropertyType propertyType = await _context.PropertyTypes.FindAsync(id);

            if (propertyType == null)
            {
                return NotFound();
            }

            return Ok(propertyType);
        }

    }
}