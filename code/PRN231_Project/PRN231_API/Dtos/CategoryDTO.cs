using System;
using System.Collections.Generic;

namespace PRN231_API.Dtos
{
    public partial class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
    }
}
