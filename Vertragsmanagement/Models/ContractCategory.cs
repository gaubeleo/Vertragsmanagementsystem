using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class ContractCategory
    {
        [Key]
        [Display(Name = "Kategorie-ID")]
        public int ID { get; set; }

        [Display(Name = "Kategorie")]
        public String name { get; set; }
    }
}
