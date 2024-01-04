using System;
using System.Collections.Generic;

namespace PRN231_API.Dtos
{
    public partial class ComicDTO
    {
        public string ComicName { get; set; }
        public IFormFile? FileImage { get; set; }
        public string Summary { get; set; }
        public int? UserId { get; set; }
        public DateTime PublicDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
