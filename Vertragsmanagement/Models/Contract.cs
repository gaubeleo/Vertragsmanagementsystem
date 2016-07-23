using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vertragsmanagement.Models
{
    public class Contract
    {

        [Key]
        [Display(Name = "Int. Nr.")]
        public int intID { get; set; }

        [Display(Name = "Ext. Nr.")]
        public int? extID { get; set; }

        [Display(Name = "Bezeichnung")]
        [AlwaysRequired(ErrorMessage = "Die Vertragsbezeichnung ist immer ein Pflichtfeld!")]
        public string titel { get; set; }

        [Display(Name = "Standort")]
        public string locality { get; set; }

        [Display(Name = "Gesamtwert")]
        [Range(0.0, 1000000.0)]
        public Decimal? contractValue { get; set; }

        [Display(Name = "Kosten pro Jahr")]
        [Range(0.0, 1000000.0)]
        public Decimal? contractCosts { get; set; }

        [Display(Name = "Zahlungsintervall in Monaten")]
        [Range(0, 100)]
        public int? paymentIntervall { get; set; }

        [Display(Name = "Kündigungsfrist in Tagen")]
        [Range(0, 100)]
        public int? noticePeriod { get; set; }

        [Display(Name = "Mindestlaufzeit in Monaten"), Range(12, 1200)]
        public int? earliestNoticePeriod { get; set; }

        [Display(Name = "Auto. Vertragsverlängerung in Monaten")]
        [Range(0, 100)]
        public int? contractExtension { get; set; }

        [Display(Name = "Mehrwertsteuer")]
        [Range(0.0, 100.0)]
        public Decimal? valueTax { get; set; }

        [Display(Name = "Vertragspartner ist ...")]
        public PartnerIs? partnerIs { get; set; }

        [Display(Name = "Bemerkungen")]
        public String remark { get; set; }

        [Display(Name = "Status")]
        public State state { get; set; }

        [Display(Name = "Kategorie")]
        public virtual ContractCategory category { get; set; }
        [ForeignKey("category")]
        [RequiredIfDispatcher(ErrorMessage = "Die Kategorie ist für Sie, als Dispatcher ein Pflichtfeld!")]
        public int? categoryID { get; set; }

        [Display(Name = "Unterkategorie")]
        public virtual ContractSubcategory subcategory { get; set; }
        [ForeignKey("subcategory")]
        public int? subcategoryID { get; set; }

        [Display(Name = "Vertragsart")]
        public virtual ContractSpecies species { get; set; }
        [ForeignKey("species")]
        [RequiredIfDispatcher(ErrorMessage = "Der Vertragsart ist für Sie als Dispatcher ein Pflichtfeld!")]
        public int? speciesID { get; set; }

        [Display(Name = "Unterzeichner")]
        public virtual User signer { get; set; }
        [ForeignKey("signer")]
        [AlwaysRequired(ErrorMessage = "Der Unterzeichner ist immer ein Pflichtfeld")]
        public int? signerID { get; set; }

        [Display(Name = "sachlicher Verantwortlicher")]
        public virtual User personInCharge { get; set; }
        [ForeignKey("personInCharge")]
        [AlwaysRequired(ErrorMessage = "Der sachliche Verantworliche ist immer ein Pflichtfeld")]
        public int? personInChargeID { get; set; }

        [Display(Name = "zugeordnete Abteilung")]
        public virtual Department mappedDepartment { get; set; }
        [ForeignKey("mappedDepartment")]
        public int? mappedDepartmentID { get; set; }

        [Display(Name = "überwachende Abteilung")]
        public virtual Department observingDepartment { get; set; }
        [ForeignKey("observingDepartment")]
        public int? observingDepartmentID { get; set; }

        [Display(Name = "Vertragspartner")]
        public virtual ContractPartner partner { get; set; }
        [ForeignKey("partner")]
        public String partnerID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Zahlungsbeginn")]
        public DateTime? paymentBegin { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Vertragsbeginn")]
        [RequiredIfDispatcher(ErrorMessage = "Der Vertragsbeginn ist für Sie, als Dispatcher ein Pflichtfeld!")]
        public DateTime? startDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Erstelldatum")]
        public DateTime? creationDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Kündigungsdatum")]
        public DateTime? noticeDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Vertragsende")]
        [AfterDate("startDate", ErrorMessage = "Das Vertragsende muss nach dem Vertragsbeginn liegen!")]
        public DateTime? endDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Erinnerungsdatum")]
        [AfterDate("startDate", ErrorMessage = "Das Errinnerungsdatum muss nach dem Vertragsbeginn liegen!")]
        [BeforeDate("endDate", ErrorMessage = "Das Errinnerungsdatum muss vor dem Vertragsende liegen!")]
        public DateTime? remindingDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Eskalationsdatum")]
        [AfterDate("remindingDate", ErrorMessage = "Das Eskalationsdatum muss nach dem Errinnerungsdatum liegen!")]
        [BeforeDate("endDate", ErrorMessage = "Das Eskalationsdatum muss vor dem Vertragsende liegen!")]
        public DateTime? escalationDate { get; set; }

        [Display(Name = "Weitere Merkmale")]
        public virtual ICollection<OtherFeature> otherFeatures { get; set; }

        [Display(Name = "angehängte Dokumente")]
        public virtual ICollection<Document> documents { get; set; }

        [Display(Name = "Aufgaben")]
        public virtual ICollection<Task> tasks { get; set; }

        public virtual ICollection<User> authorizedUsers { get; set; }

        public virtual ICollection<User> specialAuthorization { get; set; }

        public virtual ICollection<Contract> subContracts { get; set; }

        [ForeignKey("frameworkContract")]
        public int? frameworkContractID { get; set; }
        public virtual Contract frameworkContract { get; set; }

        [Display(Name = "Kostenstellen")]
        public virtual ICollection<CostCentreDivide> costCenterDivides { get; set; }

        public Boolean isframeworkContract { get; set; }

        public Boolean escalated { get; set; }

        public Boolean reminded { get; set; }

        public Boolean noticeReminded { get; set; }

        #region Task related stuff

        /// <summary>
        /// adds a task to the contract
        /// </summary>
        /// <param name="description">the description of the task</param>
        /// <param name="users">the users of the task</param>
        /// <param name="deadline">a date regarding th task</param>
        /// <returns>true if succesful else false</returns>
        public bool AddTask(string description, List<User> users, DateTime deadline)  //use db.tasks.add instead
        {
            if (tasks == null)
            {
                tasks = new List<Task>();
            }
            if (tasks.Count > 4) return false;  //in case Manager creates to many tasks, this will stop it
            Task task = new Task(intID, description, deadline, users);
            tasks.Add(task);
            return true;
        }
        /// <summary>
        /// Creates a Dictionary of Users and their roles, the RecipientType determines which roles shall be included.
        /// It also includes a multitude of measures to prevent the Dictionary from being empty. 
        /// It achieves that by getting other recipients, if the choosen Recipients don't exist. 
        /// For Example, if you want to get all Dispatchers, but there are none, it will return the Person in Charge of the Contract.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Dictionary<User, string> getRecipients(RecipientType r)
        {

            Dictionary<User, string> users = new Dictionary<User, string>();
            if (r == RecipientType.Everyone)
            {
                if (observingDepartment != null && observingDepartment.mandant != null && observingDepartment.mandant.coordinators != null)
                {
                    foreach (User c in observingDepartment.mandant.coordinators)
                    {
                        users.Add(c, "Koordinator");
                    }
                }
                if (observingDepartment != null && observingDepartment.dispatchers != null)
                {
                    foreach (User c in observingDepartment.dispatchers)
                    {
                        try { users.Add(c, "Dispatcher"); }
                        catch (Exception) { }
                    }
                }
                if (personInCharge != null)
                {
                    try
                    {
                        users.Add(personInCharge, "Sachlicher Verantwortlicher");
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (signer != null)
                {
                    try
                    {
                        users.Add(signer, "Unterschreiber");
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else if (r == RecipientType.dispatchers)
            {
                if (observingDepartment != null && observingDepartment.dispatchers != null)
                {
                    foreach (User d in observingDepartment.dispatchers)
                    {
                        users.Add(d, "Dispatcher");
                    }
                }
                else if (personInCharge != null)
                {
                    users.Add(personInCharge, "Sachlicher Verantwortlicher");
                }
            }
            else if (r == RecipientType.personInCharge)
            {
                if (personInCharge != null)
                {
                    users.Add(personInCharge, "Sachlicher Verantwortlicher");
                }
                else if (signer != null)
                {
                    users.Add(signer, "Unterschreiber");
                }
            }

            return users;
        }
        /// <summary>
        /// Contains all the possible types of Recipients a mail might be directed to
        /// </summary>
        public enum RecipientType
        {
            Everyone,  //Escalation, 
            dispatchers, //Reminder
            personInCharge //observingDepartent, reminder if dispatchers == null
        }
        /// <summary>
        /// This function returns true, if the Contract endDate if in the Past or today and false if it ist in the future.
        /// But after returning true once, it will return false when executed again.
        /// </summary>
        /// <returns></returns>
        public Boolean isEnding()
        {
            if (!endDate.HasValue) return false;
            if (!state.Equals(State.beendet) && DateTime.Now.CompareTo(endDate.Value) >= 0)
            {
                state = State.beendet;
                return true;
            }
            return false;
        }
        /// <summary>
        /// creates noticedate from notieceperiod
        /// </summary>
        /// <returns>returns noticedate or null</returns>
        private DateTime? getNoticeDate()
        {
            if (earliestNoticePeriod.HasValue)
            {
                TimeSpan span = new TimeSpan(earliestNoticePeriod.Value, 0, 0, 0);
                return startDate.Value.Add(span);
            }
            return null;
        }
        /// <summary>
        /// checks whether contract is escalating, will set escalated on true
        /// </summary>
        /// <returns>true if escalating else false</returns>
        public Boolean isEscalating()
        {
            if (!escalationDate.HasValue) return false;
            if (DateTime.Now.CompareTo(escalationDate.Value) >= 0 & !escalated)  //überprüft ob jetzt-zeit nach dem Datum liegt und ob es schon eskalierte
            {
                escalated = true;

                return true;
            }
            return false;
        }
        /// <summary>
        /// checks whether contract is reminding, will set reminded on true
        /// </summary>
        /// <returns>true if reminding else false</returns>
        public Boolean isReminding()
        {
            if (!remindingDate.HasValue) return false;
            if (DateTime.Now.CompareTo(remindingDate.Value) >= 0 & !reminded)
            {
                reminded = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// checks whether contract is noticing, will set noticed on true
        /// </summary>
        /// <returns>true if noticing else false</returns>
        public Boolean isNoticing()
        {
            if (getNoticeDate() == null) return false;
            if (DateTime.Now.CompareTo(getNoticeDate().Value) >= 0 & !noticeReminded)
            {
                noticeReminded = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// checks whether contract is completed
        /// </summary>
        /// <param name="db">the database of the contract</param>
        /// <returns>true if comleted else false</returns>
        public Boolean isCompleted(ContractDBContext db)
        {
            if (documents == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.documents).Load();
                }
                catch (Exception e)
                {
                    documents = new List<Document>();
                }
            }
            bool complete = documents.Count() > 0 &&
                startDate.HasValue &&
                titel != null &&
                categoryID != null &&
                speciesID != null &&
                mappedDepartmentID != null &&
                observingDepartmentID != null &&
                personInChargeID != null &&
                signerID != null;
            return complete;
        }

        #endregion

        public void assignUsers(ContractDBContext db)
        {
            List<User> users = db.Users.Include(u => u.department).Include(u => u.viewableContracts).
                Include(u => u.dispatcherOfDepartment).Include(u => u.coordinatorOfMandant).
                Include(u => u.coordinatorOfDepartment).ToList();
            if (authorizedUsers == null)
            {
                try
                {
                    db.Entry(this).Collection(c => c.authorizedUsers).Load();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    authorizedUsers = new List<User>();
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
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.Source);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);
                    specialAuthorization = new List<User>();
                }
            }
            if (observingDepartment == null && observingDepartmentID != null)
            {
                observingDepartment = db.Departments.Find(observingDepartmentID);
            }
            if (mappedDepartment == null && mappedDepartmentID != null)
            {
                mappedDepartment = db.Departments.Find(mappedDepartmentID);
            }
            if (signer == null && signerID != null)
            {
                signer = db.Users.Find(signerID);
            }
            if (personInCharge == null && personInChargeID != null)
            {
                personInCharge = db.Users.Find(personInChargeID);
            }

            authorizedUsers.Clear();

            foreach (User u in users)
            {
                if (u != null)
                {
                    if (u.viewableContracts == null)
                    {
                        try
                        {
                            db.Entry(u).Collection(c => c.viewableContracts).Load();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(e.Source);
                            System.Diagnostics.Debug.WriteLine(e.StackTrace);
                            u.viewableContracts = new List<Contract>();
                        }
                    }
                    if (u.coordinatorOfDepartment == null)
                    {
                        try
                        {
                            db.Entry(u).Collection(c => c.coordinatorOfDepartment).Load();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(e.Source);
                            System.Diagnostics.Debug.WriteLine(e.StackTrace);
                            u.coordinatorOfDepartment = new List<Department>();
                        }
                    }
                    if (u.coordinatorOfMandant == null)
                    {
                        try
                        {
                            db.Entry(u).Collection(c => c.coordinatorOfMandant).Load();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(e.Source);
                            System.Diagnostics.Debug.WriteLine(e.StackTrace);
                            u.coordinatorOfMandant = new List<Mandant>();
                        }
                    }
                    if (u.dispatcherOfDepartment == null)
                    {
                        try
                        {
                            db.Entry(u).Collection(c => c.dispatcherOfDepartment).Load();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(e.Source);
                            System.Diagnostics.Debug.WriteLine(e.StackTrace);
                            u.dispatcherOfDepartment = new List<Department>();
                        }
                    }

                    Boolean userShouldBeAuthorized =
                       ((u.departmentID != null && u.departmentID == mappedDepartmentID) ||
                       (mappedDepartment != null && u.coordinatorOfDepartment.Contains(mappedDepartment)) ||
                       (observingDepartment != null && u.dispatcherOfDepartment.Contains(observingDepartment)) ||
                       (mappedDepartment != null && mappedDepartment.mandant != null && u.coordinatorOfMandant.Contains(mappedDepartment.mandant)) ||
                       u.ID == signerID ||
                       u.ID == personInChargeID);

                    if (userShouldBeAuthorized || specialAuthorization.Contains(u))
                    {
                        authorizedUsers.Add(u);
                        if (!u.viewableContracts.Contains(this))
                        {
                            u.viewableContracts.Add(this);
                        }
                        db.Entry(this).State = EntityState.Modified;
                        db.Entry(u).State = EntityState.Modified;
                    }
                    else if (u.viewableContracts.Contains(this))
                    {
                        u.viewableContracts.Remove(this);
                        db.Entry(u).State = EntityState.Modified;
                    }
                }
            }
        }
    }

    public enum State
    {
        aktiv, unvollständig, gekündigt, beendet, gelöscht
    }

    public enum PartnerIs
    {
        Kreditor, Debitor
    }

    public class ContractDBContext : DbContext
    {

        public ContractDBContext() : base("name = ContractDBContext")
        {

        }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractCategory> ContractCategories { get; set; }
        public DbSet<ContractSubcategory> ContractSubcategories { get; set; }
        public DbSet<ContractPartner> ContractPartners { get; set; }
        public DbSet<ContractSpecies> ContractSpecies { get; set; }
        public DbSet<CostCentre> CostCentres { get; set; }
        public DbSet<CostCentreDivide> CostCentreDivide { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Mandant> Mandant { get; set; }
        public DbSet<OtherFeature> OtherFeatures { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>()
                .HasMany(c => c.authorizedUsers).WithMany(u => u.viewableContracts)
                .Map(t => t.MapLeftKey("ContractID")
                    .MapRightKey("UserID")
                    .ToTable("UserContractAuthorization"));

            modelBuilder.Entity<Department>()
                .HasMany(d => d.coordinators).WithMany(c => c.coordinatorOfDepartment)
                .Map(t => t.MapLeftKey("DepartmentID")
                    .MapRightKey("CoordinatorID")
                    .ToTable("DepartmentCoordinators"));

            modelBuilder.Entity<Department>()
                .HasMany(d => d.dispatchers).WithMany(d => d.dispatcherOfDepartment)
                .Map(t => t.MapLeftKey("DepartmentID")
                    .MapRightKey("DispatcherID")
                    .ToTable("DepartmentDispatchers"));

            modelBuilder.Entity<Mandant>()
                .HasMany(m => m.coordinators).WithMany(c => c.coordinatorOfMandant)
                .Map(t => t.MapLeftKey("MandantID")
                    .MapRightKey("CoordinatorID")
                    .ToTable("MandantCoordinators"));

            modelBuilder.Entity<Contract>()
                .HasMany(c => c.otherFeatures).WithMany(f => f.contractHasFeature)
                .Map(t => t.MapLeftKey("ContractID")
                    .MapRightKey("FeatureID")
                    .ToTable("ContractFeatures"));

            modelBuilder.Entity<Task>()
                .HasMany(t => t.mappedUsers).WithMany(u => u.tasks)
                .Map(t => t.MapLeftKey("TaskID")
                    .MapRightKey("UserID")
                    .ToTable("UserMappedToTask"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.specialAuthorization).WithMany(c => c.specialAuthorization)
                .Map(t => t.MapLeftKey("UserID")
                    .MapRightKey("ContractID")
                    .ToTable("SpecialAuthorization"));
        }
    }
}
