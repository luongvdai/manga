using System;
using System.Collections.Generic;

namespace PRN231_API.Dtos
{
    public partial class ChapterDTO
    {
        public int? ChapterId { get; set; }
        public int ChapterNumber { get; set; }
        public DateTime? PublicDate { get; set; } = DateTime.Now;
        public bool? Status { get; set; } = true;
    }
}
