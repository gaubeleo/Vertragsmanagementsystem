using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class OtherFeature
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Bezeichnung")]
        public String title { get; set; }

        [Display(Name = "Beschreibung")]
        public String describtion { get; set; }

        public virtual ICollection<Contract> contractHasFeature { get; set; }
    }
}