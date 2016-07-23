using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viewmodel for the view where users with special rights cann be added to a department
    /// </summary>
    public class ResponsiblePersonsDepartmentViewModel
    {
        /// <summary>
        /// object of type department
        /// </summary>
        public Department department { get; set; }

        /// <summary>
        /// users that are dispatcher of the selected department currenty
        /// </summary>
        public ICollection<User> dispatchersOfDepartment { get; set; }
        /// <summary>
        /// users that are coordinators of the selected department currently
        /// </summary>
        public ICollection<User> coordinatorsOfDepartment { get; set; }
        /// <summary>
        /// list of all users that are allowed to serve as dispatcher
        /// </summary>
        public ICollection<User> allDispatchers { get; set; }
        /// <summary>
        /// list of all users that are allowed to serve as coordinator
        /// </summary>
        public ICollection<User> allCoordniators { get; set; }

        /// <summary>
        /// fill the above defined lists
        /// </summary>
        /// <param name="db">Database context</param>
        public void PopulateLists(ContractDBContext db)
        {
            dispatchersOfDepartment = department.dispatchers;
            coordinatorsOfDepartment = department.coordinators;
            allDispatchers = db.Users.Where(u => u.dispatcher == true).ToList();
            List<User> DispatchersToRemoveFromList = new List<User>();
            foreach (User d in allDispatchers)
            {
               foreach(User dd in dispatchersOfDepartment)
                {
                    if(d.ID == dd.ID)
                    {
                        DispatchersToRemoveFromList.Add(d);
                    }
                }
            }
            foreach (User d in DispatchersToRemoveFromList)
            {
                allDispatchers.Remove(d);
            }
            allCoordniators = db.Users.Where(u => u.coordinator == true).ToList();
            List<User> CoordinatorsToRemoveFromList= new List<User>();
            foreach (User c in allCoordniators)
            {
                foreach (User cd in coordinatorsOfDepartment)
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