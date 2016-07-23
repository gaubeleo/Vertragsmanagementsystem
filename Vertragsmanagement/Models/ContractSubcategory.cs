using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class ContractSubcategory
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Unterkategorie")]
        public String name { get; set; }

        [Display(Name = "Oberkategorie")]
        public virtual ContractCategory category { get; set; }
        public int? categoryID { get; set; }
    }
}
