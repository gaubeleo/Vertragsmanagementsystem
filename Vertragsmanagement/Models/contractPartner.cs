using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class ContractPartner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Kontonummer")]
        public String accountNumbre { get; set; }

        [Display(Name = "Vertragspartner")]
        public String name { get; set; }
    }
}