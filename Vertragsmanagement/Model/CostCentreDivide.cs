//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vertragsmanagement.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CostCentreDivide
    {
        public int ID { get; set; }
        public int contractID { get; set; }
        public int costCentreID { get; set; }
        public float percentage { get; set; }
        public Nullable<int> contract_intID { get; set; }
    
        public virtual Contract Contract { get; set; }
        public virtual CostCentre CostCentre { get; set; }
    }
}
