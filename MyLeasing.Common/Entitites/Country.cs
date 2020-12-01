using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Common.Entitites
{
    public class Country
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}
