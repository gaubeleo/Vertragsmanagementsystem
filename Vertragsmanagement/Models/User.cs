using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Vertragsmanagement.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Vorname")]
        public String name { get; set; }

        [Display(Name = "Nachname")]
        public String surname { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get { return surname + ", " + name; }
        }

        [Display(Name = "E-Mail Adresse")]
        public String email { get; set; }

        [Display(Name = "AD-Anmeldename")]
        public String activeDirectoryName { get; set; }

        [Display(Name = "Abteilung")]
        public virtual Department department { get; set; }
        public int? departmentID { get; set; }

        [Display(Name = "Aufgaben")]
        public virtual ICollection<Task> tasks { get; set; }

        public virtual ICollection<Contract> viewableContracts { get; set; }

        [Display(Name = "Dispatcher von folgenden Abteilungen: ")]
        public virtual ICollection<Department> dispatcherOfDepartment { get; set; }

        public virtual ICollection<Department> coordinatorOfDepartment { get; set; }

        public virtual ICollection<Mandant> coordinatorOfMandant { get; set; }

        public virtual ICollection<Contract> specialAuthorization { get; set; }

        [Display(Name = "Administrator")]
        public Boolean administrator { get; set; } = false;

        [Display(Name = "Dispatcher")]
        public Boolean dispatcher { get; set; } = false;

        [Display(Name = "Koordinator")]
        public Boolean coordinator { get; set; } = false;

        [Display(Name = "Unterzeichner")]
        public Boolean signer { get; set; } = false;

        [Display(Name = "Sachlicher Verantwortlicher")]
        public Boolean personInCharge { get; set; } = false;

        [Display(Name = "gesperrt")]
        public Boolean blocked { get; set; } = false;

        public string getEmail()
        {
            try
            {
                return email;
               
            }
            catch (Exception e)
            {
                return activeDirectoryName.Split('#')[1];
            }
        }

        /// <summary>
        /// assign the user as dispatcher, koordinator, person in charge or signer for a contract
        /// </summary>
        /// <param name="db"></param>
        public void assignContracts(ContractDBContext db)
        {
            List<Contract> contracts = db.Contracts.Include(c => c.authorizedUsers).Include(c => c.observingDepartment).
                Include(c => c.mappedDepartment).ToList();
            if (viewableContracts == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.viewableContracts).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    viewableContracts = new List<Contract>();
                }
            }
            if (coordinatorOfDepartment == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.coordinatorOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    coordinatorOfDepartment = new List<Department>();
                }
            }
            if (coordinatorOfMandant == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.coordinatorOfMandant).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    coordinatorOfMandant = new List<Mandant>();
                }
            }
            if (dispatcherOfDepartment == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.dispatcherOfDepartment).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    dispatcherOfDepartment = new List<Department>();
                }
            }
            if (specialAuthorization == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.specialAuthorization).Load();
                }
                catch (Exception e)
                {
                    specialAuthorization = new List<Contract>();
                }
            }
            if (department == null && departmentID != null)
            {
                department = db.Departments.Find(departmentID);
            }

            viewableContracts.Clear();
           
            foreach (Contract c in contracts)
            {
                if (c != null && c.state != State.gelöscht)
                {
                    if (c.authorizedUsers == null)
                    {
                        try
                        {
                            db.Entry(c).Collection(u => u.authorizedUsers).Load();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(e.Source);
                            System.Diagnostics.Debug.WriteLine(e.StackTrace);
                            c.authorizedUsers= new List<User>();
                        }
                    }
                    if (c.observingDepartment == null && c.observingDepartmentID != null)
                    {
                        c.observingDepartment = db.Departments.Find(c.observingDepartmentID);
                    }
                    if (c.mappedDepartment == null && c.mappedDepartmentID != null)
                    {
                        c.mappedDepartment = db.Departments.Find(c.mappedDepartmentID);
                    }

                    Boolean contractShouldBeViewable =
                       ((c.mappedDepartmentID != null && departmentID == c.mappedDepartmentID) ||
                       (c.mappedDepartment != null && coordinatorOfDepartment.Contains(c.mappedDepartment)) ||
                       (c.observingDepartment != null && dispatcherOfDepartment.Contains(c.observingDepartment)) ||
                       (c.mappedDepartment != null && c.mappedDepartment.mandant != null && coordinatorOfMandant.Contains(c.mappedDepartment.mandant)) ||
                       ID == c.signerID ||
                       ID == c.personInChargeID);

                    if (contractShouldBeViewable || specialAuthorization.Contains(c))
                    {
                        viewableContracts.Add(c);
                        if (!c.authorizedUsers.Contains(this))
                        {
                            c.authorizedUsers.Add(this);
                        }
                        db.Entry(this).State = EntityState.Modified;
                        db.Entry(c).State = EntityState.Modified;
                    }
                    else if (c.authorizedUsers.Contains(this))
                    {
                        c.authorizedUsers.Remove(this);
                        db.Entry(c).State = EntityState.Modified;
                    }
                }
            }
        }
    }
}