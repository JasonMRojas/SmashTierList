using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TierList.Models
{
    public class Image
    {
        //[RegularExpression(@"http://([\w-] + \.) + [\w-] +(/[\w-./]*)+\.(?:gif|jpg|jpeg|png|bmp|GIF|JPEG|PNG|BMP|GIF|Jpg|Jpeg|Png|Bmp)$", ErrorMessage = "Invalid Image Address")]
        public string ImagePath { get; set; }

        public int OrderValue { get; set; }
    }
}