using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Vertragsmanagement.Models;
using System.Web.Mvc;

namespace Vertragsmanagement.ViewModels
{
    public class ReportContractsViewModel
    {

        public Contract contract { get; set; }

        private User currentUser { get; set; }

        public List<ReportContent> rcc = new List<ReportContent>();

        public List<ContractCategory> categoriesList = new List<ContractCategory>();
        public List<Mandant> mandantenList = new List<Mandant>();
        public List<ContractSubcategory> subcategoryList = new List<ContractSubcategory>();
        public List<ContractSpecies> speciesList = new List<ContractSpecies>();
        public List<CostCentre> costCentreList = new List<CostCentre>();
        public List<String> costTypeList = new List<String>();

        public IEnumerable<ContractCategory> categories { get; set; }
        public IEnumerable<ContractSpecies> species { get; set; }
        public IEnumerable<ContractSubcategory> subcategories { get; set; }
        public IEnumerable<Mandant> mandanten { get; set; }
        public IEnumerable<CostCentre> costCentres { get; set; }
        public IEnumerable<Contract> contracts { get; set; }
        public IEnumerable<CostCentreDivide> divide { get; set; }


        //id of parameters selected by the user; default value is 0
        public int? currentSelectedMandant { get; set; }
        public int? currentSelectedCostCentre { get; set; }
        public int? currentSelectedCategory { get; set; }
        public int? currentSelectedSubcategory { get; set; }
        public int? currentSelectedType { get; set; }
        public int? currentSelectedCostType { get; set; }

        Boolean isMandant = true;
        Boolean isCategory = true;
        Boolean isSubcategory = true;
        Boolean isSpecies = true;
        Boolean isCostCentre = true;
        Boolean isCostOrRev = true;

        /// <summary>
        /// sum of all contracts that fulfill the user-provided parameters and are costs
        /// </summary>
        public float gesSum = 0;
        /// <summary>
        /// sum of all contracts that fulfill the user-provided parameters and are revenues
        /// </summary>
        public float gesRev = 0;
        /// <summary>
        /// difference between costs and revenues
        /// </summary>
        public float gesamt = 0;
        /// <summary>
        /// Konstruktor: konvertiert die Nullable-Werte der View in echte int-Werte und läd alle notwendigen Daten
        /// </summary>
        public ReportContractsViewModel(User currentUser, int? mandant, int? costCentre, int? category, int? subcategory, int? type, int? costType,  ContractDBContext db)
        {

            //create the dropdown lists
            ContractCategory categ = new ContractCategory();
            categ.ID = 0;
            categ.name = "Keine Kategorie gewählt";
            categoriesList.Add(categ);
            categoriesList.AddRange(db.ContractCategories.OrderBy(c => c.name));

            Mandant mand = new Mandant();
            mand.name = "Kein Mandant gewählt";
            mand.ID = 0;
            mandantenList.Add(mand);
            mandantenList.AddRange(db.Mandant.OrderBy(c => c.name));

            ContractSubcategory subcateg = new ContractSubcategory();
            subcateg.name = "Keine Unterkategorie gewählt";
            subcateg.ID = 0;
            subcategoryList.Add(subcateg);
            subcategoryList.AddRange(db.ContractSubcategories.OrderBy(c => c.name));

            ContractSpecies spec = new ContractSpecies();
            spec.name = "Keine Vertragsart gewählt";
            spec.ID = 0;
            speciesList.Add(spec);
            speciesList.AddRange(db.ContractSpecies.OrderBy(c => c.name));

            CostCentre costCen = new CostCentre();
            costCen.ID = 0;
            costCen.describtion = "Keine Kostenstelle gewählt";
            costCentreList.Add(costCen);
            costCentreList.AddRange(db.CostCentres.OrderBy(c => c.describtion));

            costTypeList.Add("Kein Kostentyp gewählt");
            costTypeList.Add("Ausgabe");
            costTypeList.Add("Einnahme");

            this.currentUser = currentUser;

            //change nullable-values to int
            if(mandant==null)
            {
                mandant = 0;
                isMandant = false;
            }
            if(costCentre==null)
            {
                costCentre = 0;
                isCostCentre = false;
            }
            if(category==null)
            {
                category = 0;
                isCategory = false;
            }
            if (subcategory ==null){
                subcategory = 0;
                isSubcategory = false;
            }
            if (type == null)
            {
                type = 0;
                isSpecies = false;
            }
            if(costType == null)
            {
                costType = -1;
                isCostOrRev = false;
            }
            //set current selected parameters
            currentSelectedMandant = mandant;
            currentSelectedCostCentre = costCentre;
            currentSelectedCategory = category;
            currentSelectedSubcategory = subcategory;
            currentSelectedType = type;
            currentSelectedCostType = costType;

            fillList((int)mandant, (int)category, (int)subcategory, (int)type, (int)costCentre, db);
            getData(db, (int)costCentre, (int) costType, isMandant, isCostCentre, isCategory, isSubcategory, isSpecies, isCostOrRev);
        }


        /// <summary>
        /// Generiert Listen für jeden filterbaren Wert, in denen alle Mandanten, Kategorien, Arten und Kostenstellen drin sind, die
        /// die angegebenen Filterkriterien erfüllen
        /// </summary>
        /// <param name="mandant"></param>
        /// <param name="category"></param>
        /// <param name="subcategory"></param>
        /// <param name="type"></param>
        /// <param name="costCentre"></param>
        private void fillList(int mandant, int category, int subcategory, int type, int costCentre, ContractDBContext db)
        {
            if(type!=0)
            {
                species = speciesList.Where(s => s.ID==type);
            }
            else
            {
                species = speciesList.OrderBy(s => s.name);
            }

            if (subcategory != 0)
            {
                subcategories = subcategoryList.Where(s => s.ID==subcategory);
            }
            else
            {
                subcategories = subcategoryList.OrderBy(s => s.name);
            }

            if (category != 0)
            {
                categories = categoriesList.Where(c => c.ID == category);
            }
            else
            {
                categories = categoriesList.OrderBy(c => c.name);
            }

            if(mandant!=0)
            {
                mandanten = mandantenList.Where(m => m.ID==mandant);
            }
            else
            {
                mandanten = mandantenList.OrderBy(m => m.name);
            }
            
            if(costCentre!=0)
            {
                costCentres = costCentreList.Where(c => c.ID ==costCentre);
            }
            else
            {
                costCentres = costCentreList;
            }
            contracts = db.Contracts;
            divide = db.CostCentreDivide;
            
        }


        /// <summary>
        /// fills the lists containing all relevant contracts with their mandants, costCentres, species, categories, subcategories and costs
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selCostCentre"></param>
        /// <param name="costType"></param>
        private void getData(ContractDBContext db, int selCostCentre, int costType, Boolean isMandant, Boolean isCostCentre, Boolean isCategory, Boolean isSubcategory, Boolean isSpecies, Boolean isCostOrRev)
        {
            List<CostCentreDivide> div = divide.ToList();
            List<Mandant> mand = db.Mandant.ToList();
            //iterate over all contracts
            foreach (Contract c in contracts)
            {
                //Werte nur die Verträge aus, die den Filterkriterien entsprechen
                if (((c.species == null && !isSpecies) || (species.Contains(c.species))) && (((c.category == null && !isCategory) || (categories.Contains(c.category)))) && (((c.subcategory == null && !isSubcategory) || (subcategories.Contains(c.subcategory)))) && c.state!=State.gelöscht)
                {

                    String partnerIs;
                    try
                    {
                        partnerIs = c.partnerIs.Value.ToString(); //check if partner of current contract is debitor or creditor
                    }
                    catch (Exception)
                    {
                        partnerIs = "Kreditor";
                    }
                    int? manID = null;
                    try
                    {
                        manID = c.mappedDepartment.mandantID;  //try to get the mandant ID of current contract
                    }
                    catch (Exception)
                    {

                    }
                    Mandant man = null; //if mandant ID != 0, try to get the associated mandant to the id
                    if (manID != null)
                    {
                        try
                        {
                            man = (Mandant)mand.Where(m => m.ID == manID).First();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (mandanten.Contains(man) || (man == null && !isMandant))
                    {
                        ReportContent reportCategory = new ReportContent();
                        ContractCategory cat = c.category;
                        ContractSubcategory subCat = c.subcategory;
                        ContractSpecies spe = c.species;
                        decimal? v = c.contractValue;
                        float value = 0;
                        if (v != null)
                        {
                            value = (float)v;
                        }
                        reportCategory.category = cat;
                        if (partnerIs.Equals("Kreditor"))
                        {
                            reportCategory.costOrRev = costOrRev.Ausgabe;
                        }
                        else
                        {
                            reportCategory.costOrRev = costOrRev.Einnahme;
                        }
                        reportCategory.mandant = man;
                        reportCategory.species = spe;
                        reportCategory.subcategory = subCat;
                        reportCategory.sum = value;

                        
                        Boolean added = false;
                        foreach (CostCentreDivide cC in div.Where(d => d.contractID == c.intID))
                        {
                            ReportContent report = new ReportContent();
                            report.category = cat;
                            report.mandant = man;
                            report.species = spe;
                            report.subcategory = subCat;
                            report.costCentre = cC.costCentre;
                            report.sum = cC.percentage * value;
                            if(selCostCentre==0 || report.costCentre.ID == selCostCentre)
                            {
                                if (partnerIs.Equals("Kreditor"))
                                {
                                    report.costOrRev = costOrRev.Einnahme;
                                    if(costType==-1 || costType == 1)
                                    {
                                        if (rcc.Contains(report))
                                        {
                                            for (int i = 0; i < rcc.Count; i++)
                                            {
                                                if (rcc.ElementAt(i).equals(report))
                                                {
                                                    report.sum = report.sum + rcc.ElementAt(i).sum;
                                                    rcc.RemoveAt(i);
                                                }
                                            }
                                            rcc.Add(report);
                                            gesRev = gesRev + report.sum;
                                        }
                                        else
                                        {
                                            rcc.Add(report);
                                            gesRev = gesRev + report.sum;
                                        }
                                        added = true;
                                    }
                                }
                                else
                                {
                                    report.costOrRev = costOrRev.Ausgabe;
                                    if(costType==-1 || costType == 0)
                                    {
                                        if (rcc.Contains(report))
                                        {
                                            for (int i = 0; i < rcc.Count; i++)
                                            {
                                                if (rcc.ElementAt(i).equals(report))
                                                {
                                                    report.sum = report.sum + rcc.ElementAt(i).sum;
                                                    rcc.RemoveAt(i);
                                                }
                                            }
                                            rcc.Add(report);
                                            gesSum = gesSum + report.sum;
                                        }
                                        else
                                        {
                                            rcc.Add(report);
                                            gesSum = gesSum + report.sum;
                                        }
                                        added = true;
                                    }
                                }
                               
                            }
                        }
                        if (!added && selCostCentre==0)   //nur hinzufügen, wenn der Vertrag noch nicht hinzugefügt wurde und Konfiguration keine Kostenstellendefinition enthält
                        {
                            if(reportCategory.costOrRev.Equals(costOrRev.Einnahme) && (costType==-1 || costType == 1))
                            {
                                if (rcc.Contains(reportCategory))
                                {
                                    for (int i = 0; i < rcc.Count; i++)
                                    {
                                        if (rcc.ElementAt(i).equals(reportCategory))
                                        {
                                            reportCategory.sum = reportCategory.sum + rcc.ElementAt(i).sum;
                                            rcc.RemoveAt(i);
                                        }
                                    }
                                    rcc.Add(reportCategory);
                                    gesRev = gesRev + reportCategory.sum;
                                }
                                else
                                {
                                    rcc.Add(reportCategory);
                                    gesRev = gesRev + reportCategory.sum;
                                }
                            }
                            if (reportCategory.costOrRev.Equals(costOrRev.Ausgabe) && (costType == -1 || costType == 0))
                            {
                                if (rcc.Contains(reportCategory))
                                {
                                    for (int i = 0; i < rcc.Count; i++)
                                    {
                                        if (rcc.ElementAt(i).equals(reportCategory))
                                        {
                                            reportCategory.sum = reportCategory.sum + rcc.ElementAt(i).sum;
                                            rcc.RemoveAt(i);
                                        }
                                    }
                                    rcc.Add(reportCategory);
                                    gesSum = gesSum + reportCategory.sum;
                                }
                                else
                                {
                                    rcc.Add(reportCategory);
                                    gesSum = gesSum + reportCategory.sum;
                                }
                            }

                        }
                    }
                }
                else
                {

                }
            }
            gesamt = gesRev - gesSum;
        }
        
    }


    /// <summary>
    /// class to store one data set for report explorer
    /// </summary>
    public partial class ReportContent
    {
        /// <summary>
        /// object of type mandant
        /// </summary>
        public Mandant mandant { get; set; }
        /// <summary>
        /// object of type costCentre
        /// </summary>
        public CostCentre costCentre { get; set; }
        /// <summary>
        /// instance of enum costOrRev
        /// </summary>
        public costOrRev costOrRev { get; set; }
        /// <summary>
        /// sum of the contracts that are represented in the current object
        /// </summary>
        public float sum { get; set; }
        /// <summary>
        /// object of type category
        /// </summary>
        public ContractCategory category { get; set; }
        /// <summary>
        /// object of type subcategory
        /// </summary>
        public ContractSubcategory subcategory { get; set; }
        /// <summary>
        /// object of type species
        /// </summary>
        public ContractSpecies species { get; set;}

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportContent()
        {

        }


        /// <summary>
        /// override the default implementation of the equals method
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean equals(ReportContent other)
        {
            if(this.category.Equals(other.category) && this.subcategory.Equals(other.subcategory) && this.costOrRev.Equals(other.costOrRev) && this.costCentre.Equals(other.costCentre) && this.mandant.Equals(other.mandant) && this.species.Equals(other.species))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// enum to check if you have to pay for a contract or if you earn money with it
    /// </summary>
    public enum costOrRev
    {
        /// <summary>
        /// cost
        /// </summary>
        Ausgabe,
        /// <summary>
        /// revenue
        /// </summary>
        Einnahme
    }

    
}
