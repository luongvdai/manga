using System;
using System.Collections.Generic;

namespace PRN231_API.Models
{
    public partial class Category
    {
        public Category()
        {
            Comics = new HashSet<Comic>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Comic> Comics { get; set; }
    }
}
