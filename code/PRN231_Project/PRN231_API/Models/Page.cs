using System;
using System.Collections.Generic;

namespace PRN231_API.Models
{
    public partial class Page
    {
        public int PageId { get; set; }
        public int ChapterId { get; set; }
        public int PageNumber { get; set; }
        public byte[] Image { get; set; }

        public virtual Chapter Chapter { get; set; }
    }
}
