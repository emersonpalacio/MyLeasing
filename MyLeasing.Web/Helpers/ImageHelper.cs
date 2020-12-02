using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Helpers
{
    public class ImageHelper: IImageHelper
    {

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var patch = Path.Combine(Directory.GetCurrentDirectory(),
                  "wwwroot\\images\\Properties",
                  file);

            using (var stream = new FileStream(patch, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return $"~/images/Properties/{file}";
        }

    }
}
