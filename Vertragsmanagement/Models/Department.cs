using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class Department
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Abteilung")]
        public String name { get; set; }

        [Display(Name = "Mandant")]
        public virtual Mandant mandant { get; set; }
        [ForeignKey("mandant")]
        public int? mandantID { get; set; }

        public virtual ICollection<User> coordinators { get; set; }

        public virtual ICollection<User> dispatchers { get; set; }
    }
}