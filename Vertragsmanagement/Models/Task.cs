using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Vertragsmanagement.Models
{
    /// <summary>
    /// Contains information about curret tasks 
    /// </summary>

    public class Task
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Beschreibung")]
        public String describtion { get; set; }

        public int contractID { get; set; }
        [ForeignKey("contractID")]
        public virtual Contract contract { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Datum")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? deadline { get; set; }

        public virtual ICollection<User> mappedUsers { get; set; }

        /// <summary>
        /// Replaces all mapped users with new users, needed when Reminding changes to Escalation. 
        /// </summary>
        /// <param name="users"></param>

        public void AddUsers(List<User> users)
        {
            mappedUsers = users;
        }
        /// <summary>
        /// creates a task
        /// </summary>
        /// <param name="contractID">the id of the contract</param>
        /// <param name="description">the description of the task</param>
        /// <param name="deadline">a date regarding the task</param>
        /// <param name="users">the users this task pertains to</param>
        public Task(int contractID, string description, DateTime deadline, List<User> users)
        {
            this.deadline = deadline;
            this.contractID = contractID;
            this.describtion = description;
            this.mappedUsers = users;
            
        }

        /// <summary>
        /// Contains static strings with all suggested descriptions for a Task
        /// </summary>

        public class Description
        {
            public static String Dispatcher = "Vertrag vervollständigen";
            public static String ObservingDepartmentChoice = "Überwachende Abteilung zuzuweisen";
            public static String Escalation = "Termineskalation";
            public static String PersonInCharge = "Veranwortlicher zuzuweisen";
        }

        /// <summary>
        /// empty constructor
        /// </summary>
        public Task() { }

    }
}