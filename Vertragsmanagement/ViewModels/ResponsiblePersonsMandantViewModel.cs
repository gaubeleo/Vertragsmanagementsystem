using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viewmodel for adding a coordinator to a mandant
    /// </summary>
    public class ResponsiblePersonsMandantViewModel
    {
        /// <summary>
        /// object of type mandant
        /// </summary>
        public Mandant mandant { get; set; }

        /// <summary>
        /// users that are coordinators of the selected mandant currently
        /// </summary>
        public ICollection<User> coordinatorsOfMandant{ get; set; }
        /// <summary>
        /// users that are permitted to serve as coordinator
        /// </summary>
        public ICollection<User> allCoordniators { get; set; }

        /// <summary>
        /// fill the above defined lists
        /// </summary>
        /// <param name="db"></param>
        public void PopulateLists(ContractDBContext db)
        {
            if(mandant.coordinators == null)
            {
                db.Entry(mandant).Collection(m => m.coordinators).Load();
            }
            coordinatorsOfMandant = mandant.coordinators;
            allCoordniators = db.Users.Where(u => u.coordinator == true).ToList();
            List<User> CoordinatorsToRemoveFromList= new List<User>();
            foreach (User c in allCoordniators)
            {
                foreach (User cd in coordinatorsOfMandant)
                {
                    if (c.ID == cd.ID)
                    {
                        CoordinatorsToRemoveFromList.Add(c);
                    }
                }
            }
            foreach (User c in CoordinatorsToRemoveFromList)
            {
                allCoordniators.Remove(c);
            }
        }
    }
}