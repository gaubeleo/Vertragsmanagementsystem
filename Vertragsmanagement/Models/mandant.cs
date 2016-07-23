using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class Mandant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Buchungskreis")]
        public int ID { get; set; }

        [Display(Name = "Mandant")]
        public String name { get; set; }

        public virtual ICollection<User> coordinators { get; set; }
    }
}