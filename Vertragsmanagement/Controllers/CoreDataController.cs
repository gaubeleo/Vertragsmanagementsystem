using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vertragsmanagement.Models;
using System.Collections.Generic;
using System.Web;
using Vertragsmanagement.ViewModels;

namespace Vertragsmanagement.Controllers
{
    /// <summary>
    /// CoreData Controller
    /// </summary>
    public class CoreDataController : Controller
    {
        private ContractDBContext db = new ContractDBContext();

        //TEST Server Side Processing of Query Departments
        /// <summary>
        /// Server Side request of Cost Centres
        /// </summary>
        /// <returns></returns>
        //        public ActionResult LoadData()
        //        {
        //            using (db)
        //            {
        //                db.Configuration.LazyLoadingEnabled = false;
        //                var data = db.CostCentres.OrderBy(a => a.ID).ToList();
        //                return Json(new {data = data }, JsonRequestBehavior.AllowGet);
        //            }
        //        }

        //ContractCategorys

        /// <summary>
        /// Return Contract Categorys
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> ContractCategorys()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.ContractCategories.OrderBy(c => c.name).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create new ContractCategory
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateContractCategory()
        {
            return PartialView("ContractCategory/_Create");
        }

        /// <summary>
        /// Create new Contract Category - Controller Method
        /// </summary>
        /// <param name="contractcategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateContractCategory(ContractCategory contractcategory)
        {
            if (ModelState.IsValid)
            {
                db.ContractCategories.Add(contractcategory);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractCategory/_Create", contractcategory);
        }

        /// <summary>
        /// Delete Contract Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteContractCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractCategory contractcategory = await db.ContractCategories.FindAsync(id);
            if (contractcategory == null)
            {
                return HttpNotFound();
            }
            int contractsWithCategory = db.Contracts.Count(c => c.categoryID == id && c.state == 0);
            int subcategoriesWithCategory = db.ContractSubcategories.Count(c => c.categoryID == id);
            ViewBag.notDeletable = !(contractsWithCategory <= 0 && subcategoriesWithCategory <= 0);
            return PartialView("ContractCategory/_Delete", contractcategory);
        }

        /// <summary>
        /// Delete Contract Category - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteContractCategory")]
        public async Task<ActionResult> DeleteConfirmedContractCategory(int id)
        {
            ContractCategory contractcategory = await db.ContractCategories.FindAsync(id);
            db.ContractCategories.Remove(contractcategory);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Contract Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditContractCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractCategory contractcategory = await db.ContractCategories.FindAsync(id);
            if (contractcategory == null)
            {
                return HttpNotFound();
            }
            return PartialView("ContractCategory/_Edit", contractcategory);
        }

        /// <summary>
        /// Edit Contract Category - Controller Method
        /// </summary>
        /// <param name="contractcategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditContractCategory(ContractCategory contractcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractcategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractCategory/_Edit", contractcategory);
        }

        //=>ContractCategorys

        //ContractPartners

        /// <summary>
        /// Return Contract Partners
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> ContractPartners()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.ContractPartners.OrderBy(c => c.name).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create Contract Partner
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateContractPartner()
        {
            return PartialView("ContractPartner/_Create");
        }

        /// <summary>
        /// Create Contract Partner - Controller Method
        /// </summary>
        /// <param name="contractpartner"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateContractPartner(ContractPartner contractpartner)
        {
            if (ModelState.IsValid)
            {
                db.ContractPartners.Add(contractpartner);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractPartner/_Create", contractpartner);
        }

        /// <summary>
        /// Delete Contract Partner
        /// </summary>
        /// <param name="accountNumbre"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteContractPartner(String accountNumbre)
        {
            if (accountNumbre == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractPartner contractpartner = await db.ContractPartners.FindAsync(accountNumbre);
            if (contractpartner == null)
            {
                return HttpNotFound();
            }
            int contractsWithPartner = db.Contracts.Where(c => c.partnerID == accountNumbre && c.state == 0).Count();
            ViewBag.notDeletable = !(contractsWithPartner <= 0);
            return PartialView("ContractPartner/_Delete", contractpartner);
        }

        /// <summary>
        /// Delete Contract Partner - Controller Method
        /// </summary>
        /// <param name="accountNumbre"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteContractPartner")]
        public async Task<ActionResult> DeleteConfirmedContractPartner(String accountNumbre)
        {
            ContractPartner contractpartner = await db.ContractPartners.FindAsync(accountNumbre);
            db.ContractPartners.Remove(contractpartner);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Contract Partner
        /// </summary>
        /// <param name="accountNumbre"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditContractPartner(String accountNumbre)
        {
            if (accountNumbre == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContractPartner contractpartner = await db.ContractPartners.FindAsync(accountNumbre);
            if (contractpartner == null)
            {
                return HttpNotFound();
            }
            return PartialView("ContractPartner/_Edit", contractpartner);

        }

        /// <summary>
        /// Edit Contract Partner - Controller Method
        /// </summary>
        /// <param name="contractpartner"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditContractPartner(ContractPartner contractpartner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractpartner).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractPartner/_Edit", contractpartner);
        }

        //=>ContractPartners

        //ContractSpecies
        /// <summary>
        /// Return Contract Species
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> ContractSpecies()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.ContractSpecies.OrderBy(c => c.name).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create Contract Specie
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateContractSpecie()
        {
            return PartialView("ContractSpecies/_Create");
        }

        /// <summary>
        /// Create Contract Specie - Controller Method
        /// </summary>
        /// <param name="contractspecies"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateContractSpecie(ContractSpecies contractspecies)
        {
            if (ModelState.IsValid)
            {
                db.ContractSpecies.Add(contractspecies);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractSpecies/_Create", contractspecies);
        }

        /// <summary>
        /// Delete Contract Specie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteContractSpecies(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSpecies contractspecies = await db.ContractSpecies.FindAsync(id);
            if (contractspecies == null)
            {
                return HttpNotFound();
            }
            int contractsWithSpecies = db.Contracts.Where(c => c.speciesID == id && c.state == 0).Count();
            ViewBag.notDeletable = !(contractsWithSpecies <= 0);
            return PartialView("ContractSpecies/_Delete", contractspecies);
        }

        /// <summary>
        /// Delete Contract Specie - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteContractSpecies")]
        public async Task<ActionResult> DeleteConfirmedContractSpecies(int id)
        {
            ContractSpecies contractspecies = await db.ContractSpecies.FindAsync(id);
            db.ContractSpecies.Remove(contractspecies);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Contract Specie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditContractSpecies(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSpecies contractspecies = await db.ContractSpecies.FindAsync(id);
            if (contractspecies == null)
            {
                return HttpNotFound();
            }
            return PartialView("ContractSpecies/_Edit", contractspecies);
        }

        /// <summary>
        /// Edit Contract Specie - Controller Method
        /// </summary>
        /// <param name="contractspecies"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditContractSpecies(ContractSpecies contractspecies)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractspecies).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractSpecies/_Edit", contractspecies);
        }

        //=>ContractSpecies

        //ContractSubcategorys
        /// <summary>
        /// Return Contract Subcategory
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> ContractSubcategorys()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.ContractSubcategories.Include(s => s.category).OrderBy(c => c.name).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create Contract Subcategory
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateContractSubcategory()
        {
            ViewBag.selectCategory = new SelectList(db.ContractCategories.ToList(), "ID", "name");
            return PartialView("ContractSubcategory/_Create");
        }

        /// <summary>
        /// Create Contract Subcategory - Controller Method
        /// </summary>
        /// <param name="contractsubcategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateContractSubcategory(ContractSubcategory contractsubcategory)
        {
            if (ModelState.IsValid)
            {
                db.ContractSubcategories.Add(contractsubcategory);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("ContractSubcategory/_Create", contractsubcategory);
        }

        /// <summary>
        /// Delete Contract Subcategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteContractSubcategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSubcategory contractsubcategory = await db.ContractSubcategories.FindAsync(id);
            if (contractsubcategory == null)
            {
                return HttpNotFound();
            }
            int contractsWithSubcategory = db.Contracts.Where(c => c.subcategoryID == id && c.state == 0).Count();
            ViewBag.notDeletable = !(contractsWithSubcategory <= 0);
            return PartialView("ContractSubcategory/_Delete", contractsubcategory);
        }

        /// <summary>
        /// Delete Contract Subcategory - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteContractSubcategory")]
        public async Task<ActionResult> DeleteConfirmedContractSubcategory(int id)
        {
            ContractSubcategory contractsubcategory = await db.ContractSubcategories.FindAsync(id);
            db.ContractSubcategories.Remove(contractsubcategory);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Contract Subcategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditContractSubcategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractSubcategory contractsubcategory = await db.ContractSubcategories.FindAsync(id);
            if (contractsubcategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.selectCategory = new SelectList(db.ContractCategories.ToList(), "ID", "name", contractsubcategory.categoryID);
            return PartialView("ContractSubcategory/_Edit", contractsubcategory);
        }

        /// <summary>
        /// Edit Contract Subcategory - Controller Method
        /// </summary>
        /// <param name="contractsubcategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditContractSubcategory(ContractSubcategory contractsubcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractsubcategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.selectCategory = new SelectList(db.ContractCategories.ToList(), "ID", "name", contractsubcategory.categoryID);
            return PartialView("ContractSubcategory/_Edit", contractsubcategory);
        }

        //=>ContractSubcategorys

        //CostCentres
        /// <summary>
        /// Return Cost Centres
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> CostCentres()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.CostCentres.OrderBy(c => c.ID).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create Cost Centre
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateCostCentre()
        {
            return PartialView("CostCentre/_Create");
        }

        /// <summary>
        /// Create Cost Centre
        /// </summary>
        /// <param name="costcentre"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateCostCentre(CostCentre costcentre)
        {
            if (ModelState.IsValid)
            {
                db.CostCentres.Add(costcentre);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("CostCentre/_Create", costcentre);
        }

        /// <summary>
        /// Delete Cost Centre
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteCostCentre(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CostCentre costcentre = await db.CostCentres.FindAsync(id);
            if (costcentre == null)
            {
                return HttpNotFound();
            }
            try
            {
                int contractsWithCostCenter = db.CostCentres.SqlQuery("SELECT * FROM CostCentres WHERE ID = @p0 AND ID IN (SELECT costCentreID FROM CostCentreDivides WHERE contract_intID IN (SELECT intID FROM Contracts WHERE state = 0))", id).Count();
                ViewBag.notDeletable = !(contractsWithCostCenter <= 0);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return PartialView("CostCentre/_Delete", costcentre);
        }

        /// <summary>
        /// Delete Cost Centre - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteCostCentre")]
        public async Task<ActionResult> DeleteConfirmedCostCentre(int id)
        {
            CostCentre costcentre = await db.CostCentres.FindAsync(id);
            db.CostCentres.Remove(costcentre);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Cost Centre
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditCostCentre(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CostCentre costcentre = await db.CostCentres.FindAsync(id);
            if (costcentre == null)
            {
                return HttpNotFound();
            }
            return PartialView("CostCentre/_Edit", costcentre);

        }

        /// <summary>
        /// Edit Cost Centre - Controller Method
        /// </summary>
        /// <param name="costcentre"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditCostCentre(CostCentre costcentre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(costcentre).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("CostCentre/_Edit", costcentre);
        }

        //=>CostCentres

        //Departments
        /// <summary>
        /// Return Departments
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Departments()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.Departments.Include(d => d.mandant).OrderBy(d => d.name).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
            //Für serverseitige Abfrage
            //return View();
        }

        /// <summary>
        /// Create Department
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateDepartment()
        {
            ViewBag.selectMandant = new SelectList(db.Mandant.ToList(), "ID", "name");
            return PartialView("Department/_Create");
        }

        /// <summary>
        /// Create Department - Controller Method
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("Department/_Create", department);
        }

        /// <summary>
        /// Delete Department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteDepartment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            int usersWithDepartment = db.Users.Count(u => u.departmentID == id);
            int contractsWithDepartment = db.Contracts.Count(c => (c.observingDepartmentID == id || c.mappedDepartmentID == id));
            ViewBag.notDeletable = !(contractsWithDepartment <= 0 && usersWithDepartment <= 0);
            return PartialView("Department/_Delete", department);
        }

        /// <summary>
        /// Delete Department - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteDepartment")]

        public async Task<ActionResult> DeleteConfirmedDepartment(int id)
        {
            Department department = await db.Departments.FindAsync(id);
            db.Departments.Remove(department);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditDepartment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.selectMandant = new SelectList(db.Mandant.ToList(), "ID", "name", department.mandantID);
            return PartialView("Department/_Edit", department);

        }

        /// <summary>
        /// Edit Department - Controller Method
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.selectMandant = new SelectList(db.Mandant.ToList(), "ID", "name", department.mandantID);
            return PartialView("Department/_Edit", department);
        }

        //=>Departments

        //Mandants
        /// <summary>
        /// Return Mandants
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Mandants()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.Mandant.OrderBy(m => m.name).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create Mandant
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateMandant()
        {
            return PartialView("Mandant/_Create");
        }

        /// <summary>
        /// Create Mandant - Controller Method
        /// </summary>
        /// <param name="mandant"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateMandant(Mandant mandant)
        {
            if (ModelState.IsValid)
            {
                db.Mandant.Add(mandant);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("Mandant/_Create", mandant);
        }

        /// <summary>
        /// Delete Mandant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteMandant(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mandant mandant = await db.Mandant.FindAsync(id);
            if (mandant == null)
            {
                return HttpNotFound();
            }
            int departmentsWithMandant = db.Departments.Where(d => d.mandantID == id).Count();
            ViewBag.notDeletable = !(departmentsWithMandant <= 0);
            return PartialView("Mandant/_Delete", mandant);
        }

        /// <summary>
        /// Delete Mandant - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteMandant")]
        public async Task<ActionResult> DeleteConfirmedMandant(int id)
        {
            Mandant mandant = await db.Mandant.FindAsync(id);
            db.Mandant.Remove(mandant);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Mandant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditMandant(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mandant mandant = await db.Mandant.FindAsync(id);
            if (mandant == null)
            {
                return HttpNotFound();
            }
            return PartialView("Mandant/_Edit", mandant);
        }

        /// <summary>
        /// Edit Mandant - Controller Method
        /// </summary>
        /// <param name="mandant"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditMandant(Mandant mandant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mandant).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("Mandant/_Edit", mandant);
        }

        //=>Mandants

        //OtherFeatures
        /// <summary>
        /// Return Other Features
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> OtherFeatures()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.OtherFeatures.OrderBy(o => o.describtion).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create Other Features
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateOtherFeature()
        {
            return PartialView("OtherFeature/_Create");
        }

        /// <summary>
        /// Create Other Features - Controller Method
        /// </summary>
        /// <param name="otherfeature"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateOtherFeature(OtherFeature otherfeature)
        {
            if (ModelState.IsValid)
            {
                db.OtherFeatures.Add(otherfeature);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("OtherFeature/_Create", otherfeature);
        }

        /// <summary>
        /// Delete Other Features
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteOtherFeature(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherFeature otherfeature = await db.OtherFeatures.FindAsync(id);
            if (otherfeature == null)
            {
                return HttpNotFound();
            }
            int contractsWithFeature = db.OtherFeatures.SqlQuery("SELECT * FROM OtherFeatures WHERE ID = @p0 AND ID IN(SELECT FeatureID FROM ContractFeatures WHERE ContractID IN (SELECT intID FROM Contracts WHERE state = 0))", id).Count();
            ViewBag.notDeletable = !(contractsWithFeature <= 0);
            return PartialView("OtherFeature/_Delete", otherfeature);
        }

        /// <summary>
        /// Delete Other Features - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteOtherFeature")]
        public async Task<ActionResult> DeleteConfirmedOtherFeature(int id)
        {
            OtherFeature otherfeature = await db.OtherFeatures.FindAsync(id);
            db.OtherFeatures.Remove(otherfeature);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        /// <summary>
        /// Edit Other Features
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditOtherFeature(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OtherFeature otherfeature = await db.OtherFeatures.FindAsync(id);
            if (otherfeature == null)
            {
                return HttpNotFound();
            }
            return PartialView("OtherFeature/_Edit", otherfeature);

        }

        /// <summary>
        /// Edit Other Features - Controller Method
        /// </summary>
        /// <param name="otherfeature"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditOtherFeature(OtherFeature otherfeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(otherfeature).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("OtherFeature/_Edit", otherfeature);
        }

        //=>OtherFeatures

        //Users
        /// <summary>
        /// Return Users
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Index()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                return View(await db.Users.OrderBy(u => u.name).Include(u => u.dispatcherOfDepartment).ToListAsync());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateUser()
        {
            ViewBag.selectDepartment = new SelectList(db.Departments.ToList(), "ID", "name");
            return PartialView("User/_Create");
        }

        /// <summary>
        /// Create User - Controller Method
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                user.assignContracts(db);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("User/_Create", user);
        }

        /// <summary>
        /// Edit User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.selectDepartment = new SelectList(db.Departments.ToList(), "ID", "name", user.departmentID);
            ViewBag.personInCharge = db.Contracts.Where(c => c.personInChargeID == user.ID).Count() > 0;
            return PartialView("User/_Edit", user);
        }

        /// <summary>
        /// Edit User - Controller Method
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                if (user.dispatcher == false)
                {
                    if (user.dispatcherOfDepartment == null)
                    {
                        try
                        {
                            db.Entry(user).Collection(u => u.dispatcherOfDepartment).Load();
                        }
                        catch (Exception e)
                        {
                            user.dispatcherOfDepartment = new List<Department>();
                        }
                    }
                    if (user.dispatcherOfDepartment.Count() > 0)
                    {
                        user.dispatcherOfDepartment.Clear();
                        db.Entry(user).State = EntityState.Modified;
                    }
                }
                if (user.coordinator == false)
                {
                    if (user.coordinatorOfDepartment == null)
                    {
                        try
                        {
                            db.Entry(user).Collection(u => u.coordinatorOfDepartment).Load();
                        }
                        catch (Exception e)
                        {
                            user.coordinatorOfDepartment = new List<Department>();
                        }
                    }
                    if (user.coordinatorOfMandant == null)
                    {
                        try
                        {
                            db.Entry(user).Collection(u => u.coordinatorOfMandant).Load();
                        }
                        catch (Exception e)
                        {
                            user.coordinatorOfMandant = new List<Mandant>();
                        }
                    }
                    if (user.coordinatorOfDepartment.Count() > 0)
                    {
                        user.coordinatorOfDepartment.Clear();
                        db.Entry(user).State = EntityState.Modified;

                    }
                    if (user.coordinatorOfMandant.Count() > 0)
                    {
                        user.coordinatorOfMandant.Clear();
                        db.Entry(user).State = EntityState.Modified;
                    }
                }
                user.assignContracts(db);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("User/_Edit", user);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            int contractsWithUser = db.Contracts.Where(c => (c.signerID == id || c.personInChargeID == id)).Count();
            ViewBag.notDeletable = !(contractsWithUser <= 0);
            return PartialView("User/_Delete", user);
        }

        /// <summary>
        /// Delete User - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteUser")]
        public async Task<ActionResult> DeleteConfirmedUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }
        //=>Users

        /// <summary>
        /// Dispose override
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Import Cost Centre
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ImportCostCentre()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            return View("CostCentre/Import");
        }

        /// <summary>
        /// Import Cost Centre - Controller Method
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost, ActionName("ImportCostCentre")]
        public async Task<ActionResult> ImportCostCentreConfirmed(String path)
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (path != null)
            {
                String ending = null;
                if (path.Length >= 3)
                {
                    ending = path.Substring(path.Length - 3);
                }
                
                if (ending!=null && ending.Equals("csv"))
                {
                    var reader = new System.IO.StreamReader(System.IO.File.OpenRead(path), System.Text.Encoding.Default);
                    int failed = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        CostCentre costCentre = new CostCentre();
                        try
                        {
                            costCentre.ID = Int32.Parse(values[0]);
                            costCentre.describtion = values[1];
                            db.CostCentres.Add(costCentre);
                            await db.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            failed++;
                        }

                    }
                    return Json(new { success = true });
                }
                else
                {
                    return RedirectToAction("importFailed", "Contract");
                }
            }
            else
            {
                return RedirectToAction("importFailed", "Contract");
            }


            //return PartialView("CostCentre/_Import");
        }

        /// <summary>
        /// Responsible Persons Department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult responsiblePersonsDepartment(int id)
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                var vm = new ResponsiblePersonsDepartmentViewModel();
                vm.department = db.Departments.Include(d => d.dispatchers).Include(d => d.coordinators).Where(d => d.ID == id).Single();
                vm.PopulateLists(db);
                return View(vm);
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Responsible Persons Department - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult responsiblePersonsDepartment(int? id, int? userID, String role)
        {
            if (id == null || userID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (role == "dispatcher")
            {
                Department department = db.Departments.Find(id);
                if (department.dispatchers == null)
                {
                    db.Entry(department).Collection(d => d.dispatchers).Load();
                }
                User user = db.Users.Find(userID);
                if (user.dispatcherOfDepartment == null)
                {
                    db.Entry(user).Collection(u => u.dispatcherOfDepartment).Load();
                }
                department.dispatchers.Add(user);
                user.dispatcherOfDepartment.Add(department);
                user.assignContracts(db);
                db.Entry(department).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (role == "coordinator")
            {
                Department department = db.Departments.Find(id);
                if (department.coordinators == null)
                {
                    db.Entry(department).Collection(d => d.coordinators).Load();
                }
                User user = db.Users.Find(userID);
                if (user.coordinatorOfDepartment == null)
                {
                    db.Entry(user).Collection(u => u.coordinatorOfDepartment).Load();
                }
                department.coordinators.Add(user);
                user.coordinatorOfDepartment.Add(department);
                user.assignContracts(db);
                db.Entry(department).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("responsiblePersonsDepartment", new { id });
        }

        /// <summary>
        /// Delete Responsible Person From Department
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteResponsiblePersonFromDepartment(int? id, int? userID, String role)
        {
            if (id == null || userID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (role == "dispatcher")
            {
                Department department = db.Departments.Find(id);
                if (department.dispatchers == null)
                {
                    db.Entry(department).Collection(d => d.dispatchers).Load();
                }
                User user = db.Users.Find(userID);
                if (user.dispatcherOfDepartment == null)
                {
                    db.Entry(user).Collection(u => u.dispatcherOfDepartment).Load();
                }
                department.dispatchers.Remove(user);
                user.dispatcherOfDepartment.Remove(department);
                user.assignContracts(db);
                db.Entry(department).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (role == "coordinator")
            {
                Department department = db.Departments.Find(id);
                if (department.coordinators == null)
                {
                    db.Entry(department).Collection(d => d.coordinators).Load();
                }
                User user = db.Users.Find(userID);
                if (user.coordinatorOfDepartment == null)
                {
                    db.Entry(user).Collection(u => u.coordinatorOfDepartment).Load();
                }
                department.coordinators.Remove(user);
                user.coordinatorOfDepartment.Remove(department);
                user.assignContracts(db);
                db.Entry(department).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("responsiblePersonsDepartment", new { id });
        }

        /// <summary>
        /// Responsible Persons Mandant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult responsiblePersonsMandant(int id)
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (currentUser.administrator == true)
            {
                var vm = new ResponsiblePersonsMandantViewModel();
                vm.mandant = db.Mandant.Find(id);
                vm.PopulateLists(db);
                return View(vm);
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Responsible Persons Mandant - Controller Method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult responsiblePersonsMandant(int? id, int? userID)
        {
            if (id == null || userID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mandant mandant = db.Mandant.Find(id);
            if (mandant.coordinators == null)
            {
                db.Entry(mandant).Collection(m => m.coordinators).Load();
            }
            User user = db.Users.Find(userID);
            if (user.coordinatorOfMandant == null)
            {
                db.Entry(user).Collection(u => u.coordinatorOfMandant).Load();
            }
            mandant.coordinators.Add(user);
            user.coordinatorOfMandant.Add(mandant);
            user.assignContracts(db);
            db.Entry(mandant).State = EntityState.Modified;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("responsiblePersonsMandant", new { id });
        }

        /// <summary>
        /// Delete Responsible Person From Mandant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteResponsiblePersonFromMandant(int? id, int? userID)
        {
            if (id == null || userID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mandant mandant = db.Mandant.Find(id);
            if (mandant.coordinators == null)
            {
                db.Entry(mandant).Collection(m => m.coordinators).Load();
            }
            User user = db.Users.Find(userID);
            if (user.coordinatorOfMandant == null)
            {
                db.Entry(user).Collection(u => u.coordinatorOfMandant).Load();
            }
            mandant.coordinators.Remove(user);
            user.coordinatorOfMandant.Remove(mandant);
            user.assignContracts(db);
            db.Entry(mandant).State = EntityState.Modified;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("responsiblePersonsMandant", new { id });
        }

        /// <summary>
        /// Delete Contract
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult deletedContracts(String ErrorMessage)
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            if (ErrorMessage != null)
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = ErrorMessage;
            }
            if (currentUser.administrator == true)
            {
                return View(db.Contracts.Where(c => c.state == State.gelöscht).ToList());
            }
            else
            {
                return RedirectToAction("noAdmin");
            }
        }

        /// <summary>
        /// Restore Contract
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult restoreContract(int id)
        {
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return RedirectToAction("deletedContracts", new { ErrorMessage = "Vertrag nicht vorhanden." });
            }
            if (contract.isCompleted(db))
            {
                contract.state = State.aktiv;
            } else
            {
                contract.state = State.unvollständig;
            }
            contract.assignUsers(db);
            db.Entry(contract).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Error = false;
            return RedirectToAction("deletedContracts");
        }

        /// <summary>
        /// Authentication Denied
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult noAdmin()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            ViewBag.currentUser = currentUser.FullName;
            return View();
        }

        /// <summary>
        /// Get Current User
        /// </summary>
        /// <returns></returns>
        public User getCurrentUser()
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    return db.Users.Include(u => u.viewableContracts).FirstOrDefault(u => u.activeDirectoryName == User.Identity.Name);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}