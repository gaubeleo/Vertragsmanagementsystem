using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viewmodel for adding an additional authorized user to a contract
    /// </summary>
    public class SpecialAuthorizationViewModel
    {
        /// <summary>
        /// object of type contract
        /// </summary>
        public Contract contract { get; set; }

        /// <summary>
        /// users that are authorized to see the contract additional to the responsible departments
        /// </summary>
        public ICollection<User> SpecialAuthorizedUsersOfContract { get; set; }
        /// <summary>
        /// list of all users
        /// </summary>
        public ICollection<User> allUsers { get; set; }

        /// <summary>
        /// fill the above defined lists
        /// </summary>
        /// <param name="db"></param>
        public void PopulateLists(ContractDBContext db)
        {
            SpecialAuthorizedUsersOfContract = contract.specialAuthorization;
            allUsers = db.Users.ToList();
            List<User> UsersToRemoveFromList = new List<User>();
            foreach (User u in allUsers)
            {
                foreach (User uC in SpecialAuthorizedUsersOfContract)
                {
                    if (u.ID == uC.ID)
                    {
                        UsersToRemoveFromList.Add(u);
                    }
                }
            }
            foreach (User u in UsersToRemoveFromList)
            {
                allUsers.Remove(u);
            }
        }
    }
}