using System;
using System.Collections.Generic;

namespace PRN231_API.Models
{
    public partial class Chapter
    {
        public Chapter()
        {
            Pages = new HashSet<Page>();
        }

        public int ChapterId { get; set; }
        public int ComicId { get; set; }
        public int ChapterNumber { get; set; }
        public DateTime PublicDate { get; set; }
        public bool Status { get; set; }

        public virtual Comic Comic { get; set; }
        public virtual ICollection<Page> Pages { get; set; }
    }
}
