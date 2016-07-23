using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class CostCentre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Kostenstelle")]
        public int ID { get; set; }

        [Display(Name = "Beschreibung")]
        public String describtion { get; set; }
        [Display(Name = "Name")]
        public string Divide
        {
            get { return ID + "-" + describtion; }
        }

        public virtual ICollection<CostCentreDivide> costCenterDivides { get; set; }
    }
}
