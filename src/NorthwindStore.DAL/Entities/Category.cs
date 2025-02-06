using System;
using System.Collections.Generic;
using Riganti.Utils.Infrastructure.Core;

namespace NorthwindStore.DAL.Entities
{
    public partial class Category : IEntity<int>
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool HasPicture { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
