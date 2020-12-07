using System;
using System.Collections.Generic;
using System.Text;

namespace MyLeasing.Common.Models
{
    public  class ImageRequest
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public byte[] ImageArray { get; set; }

    }
}
