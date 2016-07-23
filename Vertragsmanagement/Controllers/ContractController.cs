using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Vertragsmanagement.Models;
using Vertragsmanagement.ViewModels;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using Vertragsmanagement.Models.Manager;
using Task = Vertragsmanagement.Models.Task;
using System.Collections;

namespace Vertragsmanagement.Controllers
{
    public class ContractController : Controller
    {
        private ContractDBContext db = new ContractDBContext();

        /// <summary>
        /// Return JSON Object of availible Contract Categories
        /// </summary>
        /// <returns></returns>
        public JsonResult ContractCategories()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json(db.ContractCategories.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return JSON Object of availible Contract Subcategories - depends on choosen Contract Category
        /// </summary>
        /// <param name="contractID"></param>
        /// <returns></returns>
        public JsonResult ContractSubcategories(string contractID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            int Id = Convert.ToInt32(contractID);
            var states = from a in db.ContractSubcategories where a.categoryID == Id select a;
            return Json(states);
        }

        /// <summary>
        /// creates the viewmodel that is needed to display data on Contract Index Page
        /// </summary>
        /// <returns>Index view with viemodel</returns>
        [Authorize]
        public ActionResult Index(String state)
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
            ViewBag.State = "";
            if (state != null && state != "")
                ViewBag.State = state;

            var vm = new IndexContractsViewModel();
            vm.contract = new Contract();
            vm.PopulateContractList(currentUser.ID, db);
            vm.PopulateDropDownLists(db);

            return View(vm);
        }

        /// <summary>
        /// initializing a viewmodel for creating a new contract
        /// </summary>
        /// <returns>Contract Create View with viewmodel or nothing of user has not the permission to create a new contract</returns>
        [Authorize]
        public ActionResult Create()
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
            if (currentUser.dispatcher || currentUser.coordinator || currentUser.personInCharge || currentUser.signer)
            {
                var vm = new ContractViewModel();
                vm.contract = new Contract();
                vm.PopulateDropDownLists(db);

                if (currentUser.signer)
                    ViewBag.isSigner = true;
                else
                    ViewBag.isSigner = false;

                return View(vm);
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }

        }


        /// <summary>
        /// This method is called when the submit button is pressed after the user created a new Contract.
        /// Initializes a new Object of type Contract an, if it´s valid, stores it to the database
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="submitButton"></param>
        /// <returns>Contract Index view with viewmodel</returns>
        [HttpPost]
        public ActionResult Create(Contract contract, String submitButton)
        {
            if (ModelState.IsValid)
            {
                contract.creationDate = DateTime.Now;
                if (contract.earliestNoticePeriod != null && contract.contractCosts != null)
                {
                    contract.contractValue = contract.contractCosts * (contract.earliestNoticePeriod / 12);
                }
                contract.state = State.unvollständig;
                db.Contracts.Add(contract);
                db.SaveChanges();

                if (contract.tasks == null)
                {
                    contract.tasks = new List<Task>();
                }
                if (contract.personInCharge == null && contract.personInChargeID != null)
                {
                    contract.personInCharge = db.Users.Find(contract.personInChargeID);
                }

                if (contract.observingDepartmentID != null)
                {
                    contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
                    new DispatcherMail(contract, db);
                    db.Tasks.Add(new Task(contract.intID, Task.Description.Dispatcher, DateTime.Now, contract.getRecipients(Contract.RecipientType.dispatchers).Keys.ToList()));
                }
                else
                {
                    new DepartmentChoiceMail(contract);
                    List<User> personInChargeList = new List<User>();
                    personInChargeList.Add(contract.personInCharge);
                    db.Tasks.Add(new Task(contract.intID, Task.Description.ObservingDepartmentChoice, DateTime.Now, personInChargeList));
                }

                contract.assignUsers(db);
                db.SaveChanges();

                if (submitButton == "ToUpload")
                {
                    return RedirectToAction("Upload", new { id = contract.intID });
                }
                return RedirectToAction("Index");
            }
            var vm = new ContractViewModel();
            vm.contract = contract;
            vm.PopulateDropDownLists(db);
            return View(vm);
        }


        /// <summary>
        /// search for contract with ID==id and load its data
        /// </summary>
        /// <param name="id">ID of the contract to show details</param>
        /// <returns>data of the selected contract to show them in the view</returns>
        [Authorize]
        public ActionResult Details(int? id)
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contract contract = db.Contracts.Find(id);

            if (contract == null)
            {
                return HttpNotFound();
            }

            var vm = new ContractViewModel();
            vm.contract = contract;
            vm.PopulateDropDownLists(db);
            return View(vm);
        }

        /// <summary>
        /// check if the current user is permitted to edit this contract and if so, try to get its data from database
        /// otherwise deny access
        /// </summary>
        int TaskID = -1;
        [Authorize]
        public ActionResult Edit(int? id, int TaskID = -1)
        {
            this.TaskID = TaskID;
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                if (currentUser.signer)
                    ViewBag.isSigner = true;
                else
                    ViewBag.isSigner = false;

                var vm = new ContractViewModel();
                vm.contract = contract;
                vm.PopulateDropDownLists(db);
                return View(vm);
            }
            return RedirectToAction("notAuthorized");
        }


        /// <summary>
        /// method is called when a contract has been edit to check if changes are valid and store it in the database again
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="submitButton"></param>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Contract contract, String submitButton, int TaskID = -1)
        {
            if (ModelState.IsValid)
            {
                if (contract.earliestNoticePeriod != null && contract.contractCosts != null)
                {
                    contract.contractValue = contract.contractCosts * (contract.earliestNoticePeriod / 12);
                }
                db.Entry(contract).State = EntityState.Modified;
                if (contract.isCompleted(db))
                {
                    contract.state = State.aktiv;
                    db.Entry(contract).State = EntityState.Modified;
                }
                else
                {
                    contract.state = State.unvollständig;
                    db.Entry(contract).State = EntityState.Modified;
                }

                contract.assignUsers(db);
                db.SaveChanges();

                if (contract.isCompleted(db) && db.Tasks.Where(m => m.contractID == contract.intID && m.describtion == Task.Description.Dispatcher).Count() != 0)
                {
                    int ID = db.Tasks.FirstOrDefault(m => m.contractID == contract.intID && m.describtion == Task.Description.Dispatcher).ID;
                    db.Tasks.Remove(db.Tasks.Find(ID));
                    db.SaveChanges();
                }

                if (contract.observingDepartmentID != null) //autoremves departmentchoice mail
                {
                    if (db.Tasks.Where(m => m.contract.intID == contract.intID && m.describtion == Task.Description.ObservingDepartmentChoice).Count() != 0)
                    {
                        db.Tasks.Add(new Task(contract.intID, Task.Description.Dispatcher, DateTime.Now, contract.getRecipients(Contract.RecipientType.dispatchers).Keys.ToList()));
                        new DispatcherMail(contract, db);
                        int ID = db.Tasks.Where(m => m.contractID == contract.intID).Where(m => m.describtion == Task.Description.ObservingDepartmentChoice).First().ID;
                        db.Tasks.Remove(db.Tasks.Where(m => m.ID == ID).First());
                        db.SaveChanges();
                    }
                }
                if (submitButton == "ToUpload")
                {
                    return RedirectToAction("Upload", new { id = contract.intID });
                }
                return RedirectToAction("Index");
            }
            var vm = new ContractViewModel();
            vm.contract = contract;
            vm.PopulateDropDownLists(db);
            return View(vm);
        }


        /// <summary>
        /// check if user has permission to delete this contract and trx to load the specified contract
        /// </summary>
        /// <param name="id">ID of the contract to delete</param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = await db.Contracts.FindAsync(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                return PartialView(contract);
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }


        /// <summary>
        /// method to delete the contract with ID==id
        /// </summary>
        /// <param name="id">ID of the contract to delete</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (ModelState.IsValid)
            {
                Contract contract = await db.Contracts.FindAsync(id);
                contract.state = State.gelöscht;
                contract.authorizedUsers.Clear();
                currentUser.viewableContracts.Remove(contract);
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(currentUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        /// <summary>
        /// get current user and navigate to Import page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Import()
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
            return View();
        }


        /// <summary>
        /// import of contracts from a csv document
        /// </summary>
        /// <param name="path">path of the csv document that contains the contracts</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import(String path)
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
            if (path == null)
            {
                String oArg = String.Format("{0}", Request.Form["selectFile"]);

                return View();
            }
            else
            {
                String ending = null;
                if (path.Length >= 3)
                {
                    ending = path.Substring(path.Length - 3);
                }
                if (ending != null && ending.Equals("csv"))
                {
                    var reader = new System.IO.StreamReader(System.IO.File.OpenRead(path), System.Text.Encoding.Default);

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');


                        Contract contract = new Contract();
                        int e;
                        if (Int32.TryParse(values[3], out e))
                        {
                            contract.extID = e;
                        }
                        ContractPartner partner = null;
                        try
                        {
                            partner = db.ContractPartners.Where(c => c.accountNumbre.Equals(values[1])).First();
                        }
                        catch (Exception)
                        {

                        }
                        if (partner != null)
                        {
                            contract.partner = partner;
                        }
                        ContractCategory cat = null;
                        try
                        {
                            cat = db.ContractCategories.Where(c => c.name.Equals(values[4])).First();
                        }
                        catch (Exception)
                        {

                        }
                        if (cat != null)
                        {
                            contract.category = cat;
                        }
                        contract.titel = values[5];
                        CostCentre cC = null;
                        try
                        {
                            cC = db.CostCentres.Where(c => c.describtion.Equals(values[6])).First();
                        }
                        catch (Exception)
                        {

                        }
                        if (cC != null)
                        {
                            CostCentreDivide div = new CostCentreDivide();
                            div.contract = contract;
                            div.percentage = 100;
                            div.costCentre = cC;
                            div.costCentreID = cC.ID;
                            contract.costCenterDivides.Add(div);
                        }
                        float r;
                        if (float.TryParse(values[7], out r))
                        {
                            contract.contractValue = (Decimal)r;
                        }
                        r = 0;
                        if (float.TryParse(values[8], out r))
                        {
                            contract.contractCosts = (Decimal)r;
                        }
                        int np;
                        if (Int32.TryParse(values[12], out np))
                        {
                            contract.noticePeriod = np;
                        }
                        contract.locality = values[14];
                        contract.remark = values[16];
                        int t;
                        if (Int32.TryParse(values[17], out t))
                        {
                            contract.valueTax = (t - 1) * 100;
                        }
                        Department mD = null;
                        try
                        {
                            mD = db.Departments.Where(d => d.name.Equals(values[13])).First();
                        }
                        catch (Exception)
                        {

                        }
                        if (mD != null)
                        {
                            contract.mappedDepartment = mD;
                            contract.mappedDepartmentID = mD.ID;
                        }
                        if (values[10].Equals("unbefristet"))
                        {

                        }
                        else
                        {
                            DateTime end;
                            if (DateTime.TryParse(values[10], out end))
                            {
                                contract.endDate = end;
                            }
                        }
                        DateTime start;
                        if (DateTime.TryParse(values[9], out start))
                        {
                            contract.startDate = start;
                        }
                        DateTime cancel;
                        if (DateTime.TryParse(values[11], out cancel))
                        {
                            contract.noticeDate = cancel;
                        }
                        User resp = null;
                        try
                        {
                            resp = db.Users.Where(u => u.activeDirectoryName.Equals(values[18])).First();
                        }
                        catch (Exception)
                        {

                        }
                        if (resp != null)
                        {
                            contract.personInCharge = resp;
                            contract.personInChargeID = resp.ID;
                        }
                        contract.state = State.unvollständig;
                        try
                        {
                            db.Contracts.Add(contract);
                            db.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            return RedirectToAction("importFailed", "Contract");
                        }

                    }
                }
                else
                {
                    return RedirectToAction("importFailed", "Contract");
                }

            }
            return RedirectToAction("Index");
        }


        /// <summary>
        /// check if there are files connected to a given contract and if so, load them
        /// </summary>
        /// <param name="id"></param>
        /// <returns>list of files connected to the contract with ID==id</returns>
        [Authorize]
        public ActionResult Files(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            if (contract.documents == null)
            {
                db.Entry(contract).Collection(c => c.documents).Load();
            }
            return View(contract);
        }


        /// <summary>
        /// delete a file that is mapped to a given contract
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public ActionResult DeleteFile(int? id, int? fileID)
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
            if (id == null || fileID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            Document doc = db.Documents.Find(fileID);
            if (contract == null || doc == null)
            {
                return HttpNotFound();
            }
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                if (contract.documents == null)
                {
                    db.Entry(contract).Collection(c => c.documents).Load();
                }
                if (contract.documents.Contains(doc))
                {
                    if (contract.documents.Count == 1 && contract.state == State.aktiv)
                    {
                        contract.state = State.unvollständig;
                    }
                    contract.documents.Remove(doc);
                    db.Documents.Remove(doc);
                    db.Entry(contract).State = EntityState.Modified;
                    db.SaveChanges();

                }
                return RedirectToAction("Files", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// load data to display the view for uploading documents that should be mapped to a contract
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Upload(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                ViewBag.ContractID = id;
                return View();
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }


        /// <summary>
        /// upload a document to the server and connect it with a contract in the database
        /// </summary>
        /// <param name="id">id of the mapped contract</param>
        /// <param name="title">user-provided title of the document</param>
        /// <param name="category">user provided category of the document</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(int? id, String title, String category)
        {
            if (Request.Files.Count <= 0)
            {
                return RedirectToAction("Upload");
            }
            else
            {
                var file = Request.Files[0];
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("contract" + id);
                if (container.CreateIfNotExists())
                {
                    container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (file != null && file.ContentLength > 0)
                {
                    CloudBlockBlob azureBlockBlob = container.GetBlockBlobReference(category + "/" + title + "_" + DateTime.Now.Millisecond + "." + file.ContentType.Split('/')[1]);
                    azureBlockBlob.UploadFromStream(file.InputStream);

                    Document doc = new Document();
                    doc.ID = 0;
                    doc.title = title;
                    doc.category = category;
                    doc.link = azureBlockBlob.Uri.ToString();
                    Contract contract = db.Contracts.Find(id);
                    contract.documents.Add(doc);
                    db.Documents.Add(doc);
                    if (contract.state != State.aktiv && contract.isCompleted(db))
                    {
                        contract.state = State.aktiv;

                        if (db.Tasks.Where(m => m.contractID == contract.intID && m.describtion == Task.Description.Dispatcher).Count() != 0)
                        {
                            int ID = db.Tasks.FirstOrDefault(m => m.contractID == contract.intID && m.describtion == Task.Description.Dispatcher).ID;
                            db.Tasks.Remove(db.Tasks.Find(ID));
                            db.SaveChanges();
                        }
                    }
                    db.Entry(contract).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Files", "Contract", new { id = id });
            }
        }


        /// <summary>
        /// load view to add another feature to a contract
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult otherFeature(int? id)
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

            var vm = new OtherFeaturesViewModel();
            vm.contract = db.Contracts.Include(c => c.otherFeatures).Where(c => c.intID == id).Single();
            vm.PopulateLists(db);
            return View(vm);

        }


        /// <summary>
        /// save a user privided feature for contract with ID ==id
        /// </summary>
        /// <param name="id">id of the contract to insert a new feature</param>
        /// <param name="featureID">id of the added feature</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult otherFeature(int? id, int? featureID)
        {
            if (id == null || featureID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                if (contract.otherFeatures == null)
                {
                    try
                    {
                        db.Entry(contract).Collection(c => c.otherFeatures).Load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.Source);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        contract.otherFeatures = new List<OtherFeature>();
                    }
                }
                OtherFeature feature = db.OtherFeatures.Find(featureID);
                if (feature.contractHasFeature == null)
                {
                    try
                    {
                        db.Entry(feature).Collection(c => c.contractHasFeature).Load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.Source);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        feature.contractHasFeature = new List<Contract>();
                    }
                }
                contract.otherFeatures.Add(feature);
                feature.contractHasFeature.Add(contract);
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(feature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("otherFeature", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// load view which shows all additional features of a contract, where they can be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="featureID"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteFeatureFromContract(int? id, int? featureID)
        {
            if (id == null || featureID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                if (contract.otherFeatures == null)
                {
                    try
                    {
                        db.Entry(contract).Collection(c => c.otherFeatures).Load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.Source);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        contract.otherFeatures = new List<OtherFeature>();
                    }
                }
                OtherFeature feature = db.OtherFeatures.Find(featureID);
                if (feature.contractHasFeature == null)
                {
                    try
                    {
                        db.Entry(feature).Collection(c => c.contractHasFeature).Load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.Source);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        feature.contractHasFeature = new List<Contract>();
                    }
                }
                contract.otherFeatures.Remove(feature);
                feature.contractHasFeature.Remove(contract);
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(feature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("otherFeature", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// load view to add costCetres to a contract or split the total value to different costCentres
        /// </summary>
        /// <param name="id">id of the contract to add a costCentre</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult costCentres(int id)
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
            var vm = new CostCentresViewModel();
            vm.contract = db.Contracts.Include(c => c.otherFeatures).Where(c => c.intID == id).Single();
            vm.PopulateLists(db);
            return View(vm);
        }

        /// <summary>
        /// store the user-provided change concerning cost centres in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="costCentreID"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult costCentres(int? id, int? costCentreID, float percentage)
        {
            if (id == null || costCentreID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                CostCentre costCentre = db.CostCentres.Find(costCentreID);
                if (contract.costCenterDivides == null)
                {
                    try
                    {
                        db.Entry(contract).Collection(c => c.costCenterDivides).Load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.Source);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        contract.costCenterDivides = new List<CostCentreDivide>();
                    }
                }
                if (costCentre.costCenterDivides == null)
                {
                    try
                    {
                        db.Entry(costCentre).Collection(c => c.costCenterDivides).Load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.Source);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        costCentre.costCenterDivides = new List<CostCentreDivide>();
                    }
                }
                CostCentreDivide ccd = new CostCentreDivide();
                ccd.contract = contract;
                ccd.contractID = contract.intID;
                ccd.costCentre = costCentre;
                ccd.costCentreID = costCentre.ID;
                ccd.percentage = percentage;
                contract.costCenterDivides.Add(ccd);
                costCentre.costCenterDivides.Add(ccd);
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(costCentre).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("costCentres", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// delete one cost centre mapped to a contract in database
        /// </summary>
        /// <param name="id">id of the contract where one cost centre should be deleted</param>
        /// <param name="costCentreDivideID">ID of the costCentreDivide object that contains all costCentres that pay for the selected contract</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteCostCentreFromContract(int? id, int? costCentreDivideID)
        {
            if (id == null || costCentreDivideID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked", "Home");
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                CostCentreDivide ccd = db.CostCentreDivide.Find(costCentreDivideID);
                CostCentre costCentre = db.CostCentres.Find(ccd.costCentreID);
                contract.costCenterDivides.Remove(ccd);
                costCentre.costCenterDivides.Remove(ccd);
                db.CostCentreDivide.Remove(ccd);
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(costCentre).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("costCentres", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// load view to add additional authorized people to a contract
        /// </summary>
        /// <param name="id">ID of the contract to add an autorized person</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult specialAuthorization(int id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                var vm = new SpecialAuthorizationViewModel();
                vm.contract = db.Contracts.Find(id);
                vm.PopulateLists(db);
                return View(vm);
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// store an additional authorized user to the database and connect him to the contract
        /// </summary>
        /// <param name="id">ID of the contract, the user is permitted to see</param>
        /// <param name="authorizedUserID">ID of the user whom is given special authorization</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult specialAuthorization(int? id, int? authorizedUserID)
        {
            if (id == null || authorizedUserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            User authorizedUser = db.Users.Find(authorizedUserID);
            contract.specialAuthorization.Add(authorizedUser);
            authorizedUser.specialAuthorization.Add(contract);
            contract.authorizedUsers.Add(authorizedUser);
            authorizedUser.viewableContracts.Add(contract);
            db.Entry(contract).State = EntityState.Modified;
            db.Entry(authorizedUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("specialAuthorization", new { id });
        }

        /// <summary>
        /// delete an additional authorized user from an contract
        /// </summary>
        /// <param name="id">ID of the contract where the user should be deleted</param>
        /// <param name="authorizedUserID">ID of the user who should be deleted from autorization list of this contract</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteSpecialAuthorizationFromContract(int? id, int? authorizedUserID)
        {
            if (id == null || authorizedUserID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            User authorizedUser = db.Users.Find(authorizedUserID);
            contract.specialAuthorization.Remove(authorizedUser);
            authorizedUser.specialAuthorization.Remove(contract);
            contract.assignUsers(db);
            db.Entry(contract).State = EntityState.Modified;
            db.Entry(authorizedUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("specialAuthorization", new { id });
        }

        /// <summary>
        /// load view to add a contract to a group of contracts(framework contract)
        /// </summary>
        /// <param name="id">ID of the contract that should be added</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult frameworkContract(int id)
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
            var vm = new FrameworkContractViewModel();
            vm.contract = db.Contracts.Find(id);
            vm.PopulateLists(db);
            return View(vm);
        }

        /// <summary>
        /// adds a contract to a framework contract and stores it in the database
        /// </summary>
        /// <param name="id">ID of the contract that serves as framework contract</param>
        /// <param name="subContractID">ID of the contract that should be added to the framework contract</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult frameworkContract(int? id, int? subContractID)
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
            if (id == null || subContractID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                Contract subContract = db.Contracts.Find(subContractID);
                contract.subContracts.Add(subContract);
                subContract.frameworkContract = contract;
                subContract.frameworkContractID = contract.intID;
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(subContract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("frameworkContract", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// delete a contract from a framework contract. The contract as whole will not be deleted, only the linking
        /// </summary>
        /// <param name="id">ID of the framework contract where a contract should be deleted</param>
        /// <param name="subContractID">ID of the contract that should be deleted from the list</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteFrameworkContractFromContract(int? id, int? subContractID)
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
            if (id == null || subContractID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract.mappedDepartment == null && contract.mappedDepartmentID != null)
            {
                contract.mappedDepartment = db.Departments.Find(contract.mappedDepartmentID);
            }
            if (contract.observingDepartment == null && contract.observingDepartmentID != null)
            {
                contract.observingDepartment = db.Departments.Find(contract.observingDepartmentID);
            }
            if (currentUser.coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfDepartment = new List<Department>();
                }
            }
            if (currentUser.coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (currentUser.dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(currentUser).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    currentUser.dispatcherOfDepartment = new List<Department>();
                }
            }
            if ((currentUser.dispatcher && contract.observingDepartmentID != null && currentUser.dispatcherOfDepartment.Contains(contract.observingDepartment)) ||
                (currentUser.coordinator && contract.mappedDepartment != null && (currentUser.coordinatorOfDepartment.Contains(contract.mappedDepartment) || currentUser.coordinatorOfMandant.Contains(contract.mappedDepartment.mandant))) ||
                (currentUser.personInCharge && contract.personInChargeID == currentUser.ID) ||
                (currentUser.signer && contract.signerID == currentUser.ID))
            {
                Contract subContract = db.Contracts.Find(subContractID);
                contract.subContracts.Remove(subContract);
                subContract.frameworkContract = null;
                subContract.frameworkContractID = null;
                db.Entry(contract).State = EntityState.Modified;
                db.Entry(subContract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("frameworkContract", new { id });
            }
            else
            {
                return RedirectToAction("notAuthorized");
            }
        }

        /// <summary>
        /// navigate to report view
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Report()
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
            return View(PopulateDropDownListsReport(null, null, null, null, null, null));
        }


        /// <summary>
        /// create a new viewmodel with given parameters
        /// </summary>
        /// <param name="mandantID"></param>
        /// <param name="costCentreID"></param>
        /// <param name="dropdowncategory"></param>
        /// <param name="dropdownsubcategory"></param>
        /// <param name="typeID"></param>
        /// <param name="costTypeID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Report(int? mandantID, int? costCentreID, int? dropdowncategory, int? dropdownsubcategory, int? typeID, int? costTypeID)
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
            return View(PopulateDropDownListsReport(mandantID, costCentreID, dropdowncategory, dropdownsubcategory, typeID, costTypeID));
        }

        /// <summary>
        /// call this method, when import from csv file failed
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult importFailed()
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
            return View();
        }



        /// <summary>
        /// method is called, when a user is trying to execute an action, which he is not allowed to
        /// </summary>
        /// <returns>access denied view</returns>
        [Authorize]
        public ActionResult notAuthorized()
        {
            User currentUser = getCurrentUser();
            ViewBag.currentUser = currentUser.FullName;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// create a viewmodel for report page with the given parameters
        /// </summary>
        /// <param name="mandant"></param>
        /// <param name="costCentre"></param>
        /// <param name="category"></param>
        /// <param name="subcategory"></param>
        /// <param name="type"></param>
        /// <param name="costType"></param>
        /// <returns>requested viewmodel</returns>
        private ReportContractsViewModel PopulateDropDownListsReport(int? mandant, int? costCentre, int? category, int? subcategory, int? type, int? costType)
        {
            User currentUser = getCurrentUser();
            ReportContractsViewModel viewModel = new ReportContractsViewModel(currentUser, mandant, costCentre, category, subcategory, type, costType, db);
            return viewModel;
        }


        /// <summary>
        /// get the user that is logged in at the moment
        /// </summary>
        /// <returns>current user</returns>
        public User getCurrentUser()
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    return db.Users.FirstOrDefault(u => u.activeDirectoryName == User.Identity.Name);
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