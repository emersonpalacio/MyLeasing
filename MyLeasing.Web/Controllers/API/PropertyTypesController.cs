using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;

namespace MyLeasing.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
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
            return _context.PropertyTypes.OrderBy(p=> p.Name);
        }

        // GET: api/PropertyTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var propertyType = await _context.PropertyTypes.FindAsync(id);

            if (propertyType == null)
            {
                return NotFound();
            }

            return Ok(propertyType);
        }

    }
}