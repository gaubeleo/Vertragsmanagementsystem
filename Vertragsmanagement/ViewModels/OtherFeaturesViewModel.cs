using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viemodel for the view where special features can be added to a contract
    /// </summary>
    public class OtherFeaturesViewModel
    {
        /// <summary>
        /// object of type contract
        /// </summary>
        public Contract contract { get; set; }

        /// <summary>
        /// list of all features that are already conneted to this contract
        /// </summary>
        public ICollection<OtherFeature> otherFeaturesOfContract { get; set; }
        /// <summary>
        /// list of all possible features
        /// </summary>
        public ICollection<OtherFeature> allOtherFeatures { get; set; }


        /// <summary>
        /// fill the above defined lists
        /// </summary>
        /// <param name="db">Database Context</param>
        public void PopulateLists(ContractDBContext db)
        {
            otherFeaturesOfContract = contract.otherFeatures;
            allOtherFeatures = db.OtherFeatures.ToList();
            List<OtherFeature> FeaturesToRemoveFromList = new List<OtherFeature>();
            foreach (OtherFeature f in allOtherFeatures)
            {
                foreach (OtherFeature fC in otherFeaturesOfContract)
                {
                    if (f.ID == fC.ID)
                    {
                        FeaturesToRemoveFromList.Add(f);
                    }
                }
            }
            foreach (OtherFeature f in FeaturesToRemoveFromList)
            {
                allOtherFeatures.Remove(f);
            }
        }
    }
}