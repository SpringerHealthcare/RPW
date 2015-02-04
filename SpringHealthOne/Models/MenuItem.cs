using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SpringHealthOne.Models
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        
        public int? ParentID { get; set; }
        [Required]
        public string Label { get; set; }
        public string URL { get; set; }
        public int? PageID { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }

        public virtual MenuItem Parent { get; set; }
        public virtual ICollection<MenuItem> Children { get; set; }
    }

    
}