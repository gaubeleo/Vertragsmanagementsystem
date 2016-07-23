using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Vertragsmanagement.Models;

namespace Vertragsmanagement.ViewModels
{
    public class IndexContractsViewModel
    {
        /// <summary>
        /// collection of all contracts
        /// </summary>
        public ICollection<Contract> contracts { get; set; }
        /// <summary>
        /// object of type contract
        /// </summary>
        public Contract contract { get; set; }
        /// <summary>
        /// object of type costCentre
        /// </summary>
        public CostCentre costcentre { get; set; }
        /// <summary>
        /// list of all possible categories
        /// </summary>
        public SelectList selectCategory { get; set; }
        /// <summary>
        /// list of all possible costCentres
        /// </summary>
        public SelectList selectCostCenter { get; set; }
        /// <summary>
        /// list of all possible mandant
        /// </summary>
        public SelectList selectMandant { get; set; }
        /// <summary>
        /// list of all possible subcategories
        /// </summary>
        public SelectList selectSubcategory { get; set; }
        /// <summary>
        /// list of all possible subcategories
        /// </summary>
        public SelectList selectSpecies { get; set; }
        /// <summary>
        /// list of all possible contract partners
        /// </summary>
        public SelectList selectPartner { get; set; }
        /// <summary>
        /// list of all users that are permitted to serve as signer
        /// </summary>
        public SelectList selectSigner { get; set; }
        /// <summary>
        /// list of all users that are permitted to serve as person in charge
        /// </summary>
        public SelectList selectPersonInCharge { get; set; }
        /// <summary>
        /// list of all departments
        /// </summary>
        public SelectList selectObservingDepartment { get; set; }
        /// <summary>
        /// list of all possible optional features that describe a contract more precisely
        /// </summary>
        public SelectList selectOtherFeatures { get; set; }
        /// <summary>
        /// list of all departments
        /// </summary>
        public SelectList selectMappedDepartment { get; set; }
        /// <summary>
        /// list of all costCentres
        /// </summary>
        public SelectList selectCostCentre { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public IndexContractsViewModel()
        {

        }

        /// <summary>
        /// get list of all contracts which are stored in the database
        /// </summary>
        /// <param name="user">current user</param>
        public void PopulateContractList(int id, ContractDBContext db)
        {
            try
            {
                contracts = db.Users.Include(u => u.viewableContracts.Select(c => c.category)).
                    Include(u => u.viewableContracts.Select(c => c.mappedDepartment)).
                    Include(u => u.viewableContracts.Select(c => c.observingDepartment)).
                    Include(u => u.viewableContracts.Select(c => c.observingDepartment.mandant)).
                    Include(u => u.viewableContracts.Select(c => c.partner)).
                    Include(u => u.viewableContracts.Select(c => c.personInCharge)).
                    Include(u => u.viewableContracts.Select(c => c.signer)).
                    Include(u => u.viewableContracts.Select(c => c.species)).
                    Include(u => u.viewableContracts.Select(c => c.subcategory)).
                    Include(u => u.viewableContracts.Select(c => c.otherFeatures)).
                    Include(u => u.viewableContracts.Select(c => c.costCenterDivides)).FirstOrDefault(u => u.ID == id).viewableContracts;
            }
            catch (Exception e)
            {
                contracts = new List<Contract>();
            }
        }

        /// <summary>
        /// initialize lists for dropdown menus for the filter options of contract index page
        /// </summary>
        /// <param name="db"></param>
        public void PopulateDropDownLists(ContractDBContext db)
        {
            selectCategory = new SelectList(db.ContractCategories.OrderBy(c => c.name), "ID", "name", contract.categoryID);
            selectCostCenter = new SelectList(db.CostCentres.SqlQuery("SELECT * FROM CostCentres WHERE ID IN (SELECT costCentreID FROM CostCentreDivides) ORDER BY describtion").AsEnumerable(), "Divide", "Divide");
            selectMandant = new SelectList(db.Mandant.OrderBy(s => s.name), "ID", "name");
            selectSubcategory = new SelectList(db.ContractSubcategories.Include(i => i.category).OrderBy(s => s.name), "ID", "name", contract.subcategoryID);
            selectSpecies = new SelectList(db.ContractSpecies.OrderBy(s => s.name), "ID", "name", contract.speciesID);
            selectPartner = new SelectList(db.ContractPartners.OrderBy(p => p.name), "accountNumbre", "name", contract.partnerID);
            selectSigner = new SelectList(db.Users.Where(u => u.signer == true).OrderBy(u => u.name), "ID", "FullName", contract.signerID);
            selectPersonInCharge = new SelectList(db.Users.Where(u => u.personInCharge == true).OrderBy(u => u.name), "ID", "FullName", contract.personInChargeID);
            selectObservingDepartment = new SelectList(db.Departments.OrderBy(d => d.name), "ID", "name", contract.observingDepartmentID);
            selectOtherFeatures = new SelectList(db.OtherFeatures.OrderBy(o => o.title), "ID", "title", contract.otherFeatures);
            selectMappedDepartment = new SelectList(db.Departments.OrderBy(d => d.name), "ID", "name", contract.mappedDepartmentID);
        }
    }
}