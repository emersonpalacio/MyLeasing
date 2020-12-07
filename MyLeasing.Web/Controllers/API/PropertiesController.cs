using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyLeasing.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropertiesController : ControllerBase
    {

        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;

        public PropertiesController(DataContext dataContext, IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            this._converterHelper = converterHelper;
        }

        [HttpPost]    
        public async Task<IActionResult> PostProperty([FromBody] PropertyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Owner owner = await _dataContext.Owners.FindAsync(request.OwnerId);
            if (owner == null)
            {
                return BadRequest("Not valid owner.");
            }

            PropertyType propertyType = await _dataContext.PropertyTypes.FindAsync(request.PropertyTypeId);
            if (propertyType == null)
            {
                return BadRequest("Not valid property type.");
            }

            Property property = new Property
            {
                Address = request.Address,
                HasParkingLot = request.HasParkingLot,
                IsAvailable = request.IsAvailable,
                Neighborhood = request.Neighborhood,
                Owner = owner,
                Price = request.Price,
                PropertyType = propertyType,
                Remarks = request.Remarks,
                Rooms = request.Rooms,
                SquareMeters = request.SquareMeters,
                Stratum = request.Stratum
            };

            _dataContext.Properties.Add(property);
            await _dataContext.SaveChangesAsync();
            //return Ok(property);
            return Ok(true);
        }

        [HttpPost]
        [Route("AddImageToProperty")]
        public async Task<IActionResult> AddImageToProperty([FromBody] ImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Property property = await _dataContext.Properties.FindAsync(request.PropertyId);
            if (property == null)
            {
                return BadRequest("Not valid property.");
            }

            string imageUrl = string.Empty;
            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                MemoryStream stream = new MemoryStream(request.ImageArray);
                string guid = Guid.NewGuid().ToString();
                string file = $"{guid}.jpg";
                string folder = "wwwroot\\images\\Properties";
                string fullPath = $"~/images/Properties/{file}";
                bool response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            PropertyImage propertyImage = new PropertyImage
            {
                ImageUrl = imageUrl,
                Property = property
            };

            _dataContext.PropertyImages.Add(propertyImage);
            await _dataContext.SaveChangesAsync();
            return Ok(propertyImage);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutProperty([FromRoute] int id, [FromBody] PropertyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest();
            }

            Property oldProperty = await _dataContext.Properties.FindAsync(request.Id);
            if (oldProperty == null)
            {
                return BadRequest("Property doesn't exists.");
            }

            PropertyType propertyType = await _dataContext.PropertyTypes.FindAsync(request.PropertyTypeId);
            if (propertyType == null)
            {
                return BadRequest("Not valid property type.");
            }

            oldProperty.Address = request.Address;
            oldProperty.HasParkingLot = request.HasParkingLot;
            oldProperty.IsAvailable = request.IsAvailable;
            oldProperty.Neighborhood = request.Neighborhood;
            oldProperty.Price = request.Price;
            oldProperty.PropertyType = propertyType;
            oldProperty.Remarks = request.Remarks;
            oldProperty.Rooms = request.Rooms;
            oldProperty.SquareMeters = request.SquareMeters;
            oldProperty.Stratum = request.Stratum;

            _dataContext.Properties.Update(oldProperty);
            await _dataContext.SaveChangesAsync();
            return Ok(oldProperty);
        }

        [HttpPost]
        [Route("DeleteImageToProperty")]
        public async Task<IActionResult> DeleteImageToProperty([FromBody] ImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PropertyImage propertyImage = await _dataContext.PropertyImages.FindAsync(request.Id);
            if (propertyImage == null)
            {
                return BadRequest("Property image doesn't exist.");
            }

            _dataContext.PropertyImages.Remove(propertyImage);
            await _dataContext.SaveChangesAsync();
            return Ok(propertyImage);
        }

    }
}
