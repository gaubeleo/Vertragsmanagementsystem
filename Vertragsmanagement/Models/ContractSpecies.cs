using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class ContractSpecies
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Vertragsart")]
        public String name { get; set; }
    }
}
