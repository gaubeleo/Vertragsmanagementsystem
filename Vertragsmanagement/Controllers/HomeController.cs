using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vertragsmanagement.Models;
using Vertragsmanagement.Models.Manager;
using System.Data;
using System.Data.Entity;
using System.Net;

namespace Vertragsmanagement.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : Controller
    {
        private ContractDBContext db = new ContractDBContext();

        /// <summary>
        /// Return Index
        /// </summary>
        /// <returns>Index View</returns>
        public ActionResult Index()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked");
            }
            else
            {
                ViewBag.active = currentUser.viewableContracts.Where(c => c.state == State.aktiv).Count();
                ViewBag.escalated = currentUser.viewableContracts.Where(c => c.escalated).Count();
                ViewBag.notCompleted = currentUser.viewableContracts.Where(c => c.state == State.unvollständig).Count();
                ViewBag.notMapped = currentUser.viewableContracts.Where(m => m.observingDepartmentID == null).Count();
                ViewBag.currentUser = currentUser.FullName;
                var tasks = db.Tasks.SqlQuery("SELECT * FROM Tasks WHERE @p0 IN (SELECT UserID FROM UserMappedToTask WHERE Tasks.ID = UserMappedToTask.taskID)", currentUser.ID).AsQueryable();
                return View(tasks.Include(t => t.contract).Include(t => t.deadline).ToList());
            }
        }

        /// <summary>
        /// Return Login
        /// </summary>
        /// <returns>Sign In View</returns>
        public ActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Return Access Denied
        /// </summary>
        /// <returns>Blocked View</returns>
        public ActionResult blocked()
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
        /// Return Help
        /// </summary>
        /// <returns>Help View</returns>
        public ActionResult Help()
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked)
            {
                return RedirectToAction("blocked");
            }
            else
            {
                ViewBag.currentUser = currentUser.FullName;
                return View();
            }
        }

        /// <summary>
        /// deletes a task
        /// </summary>
        /// <param name="ID">the id of the task</param>
        /// <returns>Index View if successful</returns>
        
        public ActionResult deleteTask(int? ID)
        {
            User currentUser = getCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            if (currentUser.blocked || ! currentUser.tasks.Contains(db.Tasks.Where(m => m.ID == ID.Value).FirstOrDefault()))
            {
                return RedirectToAction("blocked");
            }
            else
            {
                db.Tasks.Remove(db.Tasks.Where(m => m.ID == ID).First());
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Get Current User
        /// </summary>
        /// <returns>Current User</returns>
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

        #region Import
        //public ActionResult Import()
        //{
        //    ContractDBContext db = new ContractDBContext();
        //    String path = @"C:\Users\Benedict\Downloads\Kundenantworten\Kundenantworten\Kostenstellen.csv";

        //    var reader = new StreamReader(System.IO.File.OpenRead(path), , System.Text.Encoding.Default);

        //    var filePaths = path.Split('\\');
        //    if(filePaths[filePaths.Length - 1].Equals("Kostenstellen.csv"))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var values = line.Split(';');
        //            CostCentre costCentre = new CostCentre();
        //            costCentre.ID = Int32.Parse(values[0]);
        //            costCentre.describtion = values[1];
        //            db.CostCentres.Add(costCentre);
        //            db.SaveChanges();
        //        }
        //    }
        //    else if(filePaths[filePaths.Length - 1].Equals("Mandanten.csv"))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var values = line.Split(';');

        //            Mandant mandant = new Mandant();
        //            mandant.ID = Int32.Parse(values[1]);
        //            mandant.name = values[0];
        //            db.Mandant.Add(mandant);
        //            db.SaveChanges();
        //        }
        //    }else if(filePaths[filePaths.Length - 1].Equals("Vertragspartner.csv"))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var values = line.Split(';');
        //            ContractPartner partner = new ContractPartner();
        //            partner.accountNumbre = values[0];
        //            partner.name = values[1];
        //            db.ContractPartners.Add(partner);
        //            db.SaveChanges();
        //        }
        //    }
        //    else if (filePaths[filePaths.Length - 1].Equals("vertragsführendeAbteilungen.csv"))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var values = line.Split(';');

        //            Department department = new Department();
        //            department.mandantID = Int32.Parse(values[0]);
        //            department.name = values[1];
        //            db.Departments.Add(department);
        //            db.SaveChanges();
        //        }
        //    }
        //    else if (filePaths[filePaths.Length - 1].Equals("Vertragsübersicht.csv"))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var values = line.Split(';');
        //            Contract contract = new Contract();
        //            //contract.partner = values[2];
        //        }
        //    }
        //    return View();
        //} 
        #endregion
    }
}