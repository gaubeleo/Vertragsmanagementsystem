using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viemodel for adding a costCentre to a contract
    /// </summary>
    public class CostCentresViewModel
    {
        /// <summary>
        /// object of type contract
        /// </summary>
        public Contract contract { get; set; }

        /// <summary>
        /// all costCentres that are involved in the selected contract
        /// </summary>
        public ICollection<CostCentreDivide> costCentresOfContract { get; set; }
        /// <summary>
        /// list of all costCentres
        /// </summary>
        public ICollection<CostCentre> allCostCentres { get; set; }

        /// <summary>
        /// fill the above defined lists
        /// </summary>
        /// <param name="db"></param>
        public void PopulateLists(ContractDBContext db)
        {
            costCentresOfContract = contract.costCenterDivides;
            allCostCentres = db.CostCentres.ToList();
            List<CostCentre> CostCentresToRemoveFromList = new List<CostCentre>();
            foreach (CostCentre c in allCostCentres)
            {
                foreach (CostCentreDivide cC in costCentresOfContract)
                {
                    if (c.ID == cC.costCentreID)
                    {
                        CostCentresToRemoveFromList.Add(c);
                    }
                }
            }
            foreach (CostCentre c in CostCentresToRemoveFromList)
            {
                allCostCentres.Remove(c);
            }
        }
    }
}