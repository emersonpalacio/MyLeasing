using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Data.Entities
{
    public class PropertyType
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "The {0} is mandatory")]
        [MaxLength(50, ErrorMessage = "The {0} fiel can not have more than {1} characters")]
        [Display(Name = "Property Type")]
        public string Name { get; set; }

        public ICollection<PropertyType> PropertyTypes { get; set; }

    }
}
