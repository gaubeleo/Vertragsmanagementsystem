using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viewmodel for adding a contract to a framework contract
    /// </summary>
    public class FrameworkContractViewModel
    {
        /// <summary>
        /// object of type contract
        /// </summary>
        public Contract contract { get; set; }

        /// <summary>
        /// list of all contracts that are part of one framework contract
        /// </summary>
        public ICollection<Contract> SubContractsOfContract { get; set; }
        /// <summary>
        /// list of all contracts
        /// </summary>
        public ICollection<Contract> allContracts { get; set; }

        /// <summary>
        /// fill the above defined lists
        /// </summary>
        /// <param name="db"></param>
        public void PopulateLists(ContractDBContext db)
        {
            SubContractsOfContract = contract.subContracts;
            allContracts = db.Contracts.Where(c=> c.state == State.aktiv).ToList();
            List<Contract> ContractsToRemoveFromList = new List<Contract>();
            foreach (Contract c in allContracts)
            {
                foreach (Contract cC in SubContractsOfContract)
                {
                    if (c.intID == cC.intID)
                    {
                        ContractsToRemoveFromList.Add(c);
                    }
                }
            }
            foreach (Contract c in ContractsToRemoveFromList)
            {
                allContracts.Remove(c);
            }
            allContracts.Remove(contract);
        }
    }
}