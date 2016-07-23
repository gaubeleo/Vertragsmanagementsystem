using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models
{
    public class Document
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Bezeichnung")]
        public String title { get; set; }

        [Display(Name = "Kategorie")]
        public String category { get; set; }

        public String link { get; set; }
    }
}