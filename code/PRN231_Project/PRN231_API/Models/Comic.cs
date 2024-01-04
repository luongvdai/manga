using System;
using System.Collections.Generic;

namespace PRN231_API.Models
{
    public partial class Comic
    {
        public Comic()
        {
            Chapters = new HashSet<Chapter>();
            Categories = new HashSet<Category>();
            Users = new HashSet<User>();
        }

        public int ComicId { get; set; }
        public string ComicName { get; set; }
        public int? UserId { get; set; }
        public byte[] Image { get; set; }
        public string Summary { get; set; }
        public DateTime PublicDate { get; set; }
        public bool IsActive { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }

        public virtual ICollection<Category>? Categories { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}
