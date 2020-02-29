using CMS.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Batch : AuditableEntity
    {
        public int BatchId { get; set; }

        public string Name { get; set; }

        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Classes { get; set; }

        public DateTime InTime { get; set; }

        public DateTime OutTime { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public Batch()
        {
            Students = new HashSet<Student>();
        }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual Client ClientName { get; set; }
    }
}
