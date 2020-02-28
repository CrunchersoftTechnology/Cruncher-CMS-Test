﻿using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class BranchAdmin : AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AId { get; set; }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public string Name { get; set; }

        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }

        public bool Active { get; set; }

        public string ContactNo { get; set; }
    }
}
