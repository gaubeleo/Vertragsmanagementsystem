using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class CostCentreDivide
    {
        [Key]
        public int ID { get; set; }

        public int contractID { get; set; }
        public virtual Contract contract { get; set; }

        public int costCentreID { get; set; }
        public virtual CostCentre costCentre { get; set; }

        [Display(Name = "prozentuale Kosten")]
        public float percentage { get; set; }
    }
}
