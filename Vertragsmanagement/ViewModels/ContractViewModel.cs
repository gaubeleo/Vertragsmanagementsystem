using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    /// <summary>
    /// viewmodel
    /// </summary>
    public class ContractViewModel
    {
        /// <summary>
        /// Contract object
        /// </summary>
        public Contract contract { get; set; }

        /// <summary>
        /// list of all possible categories
        /// </summary>
        public SelectList selectCategory { get; set; }
        /// <summary>
        /// list of all possible subcategories
        /// </summary>
        public SelectList selectSubcategory { get; set; }
        /// <summary>
        /// list of all possible contract species
        /// </summary>
        public SelectList selectSpecies { get; set; }
        /// <summary>
        /// list of all possible partners
        /// </summary>
        public SelectList selectPartner { get; set; }
        /// <summary>
        /// list of all persons that are allowed to serve as a signer
        /// </summary>
        public SelectList selectSigner { get; set; }
        /// <summary>
        /// list of all persons that are allowed to serve as a person in charge
        /// </summary>
        public SelectList selectPersonInCharge { get; set; }
        /// <summary>
        /// list of all possible departments
        /// </summary>
        public SelectList selectObservingDepartment { get; set; }
        /// <summary>
        /// list of all possible departments
        /// </summary>
        public SelectList selectMappedDepartment { get; set; }
        /// <summary>
        /// list of all possible costCentres
        /// </summary>
        public SelectList selectCostCentre { get; set; } 

        /// <summary>
        /// initialize lists that will fill dropdown menus with data
        /// </summary>
        /// <param name="db"></param>
        public void PopulateDropDownLists(ContractDBContext db)
        {

            selectCategory = new SelectList(db.ContractCategories.OrderBy(c => c.name), "ID", "name", contract.categoryID);
            selectSubcategory = new SelectList(db.ContractSubcategories.Include(i => i.category).OrderBy(s => s.name), "ID", "name", contract.subcategoryID);
            selectSpecies = new SelectList(db.ContractSpecies.OrderBy(s => s.name), "ID", "name", contract.speciesID);
            selectPartner = new SelectList(db.ContractPartners.OrderBy(p => p.name), "accountNumbre", "name", contract.partnerID);
            selectSigner = new SelectList(db.Users.Where(u => u.signer == true).OrderBy(u => u.name), "ID", "FullName", contract.signerID);
            selectPersonInCharge = new SelectList(db.Users.Where(u => u.personInCharge == true).OrderBy(u => u.name), "ID", "FullName", contract.personInChargeID);
            selectObservingDepartment = new SelectList(db.Departments.OrderBy(d => d.name), "ID", "name", contract.observingDepartmentID);
            selectMappedDepartment = new SelectList(db.Departments.OrderBy(d => d.name), "ID", "name", contract.mappedDepartmentID);
        }

    }
}