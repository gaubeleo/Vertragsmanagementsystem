using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models.Manager
{
    /// <summary>
    /// the mail will send a text about the escalation of a date
    /// </summary>
    public class EscalationMail : Mail
    {
        Contract.RecipientType rtype;
        /// <summary>
        /// Executes the autoSend function for each user this is pertaining to.
        /// Default RecipientType is Everyone, this is used for an Escalation. The RecipientType Dispatchers is used for Reminders.
        /// </summary>
        /// <param name="contract">the contract the mail pertains to</param>
        /// <param name="rtype">the type of recipients this mail is send to</param>
        public EscalationMail(Contract contract, Contract.RecipientType rtype = Contract.RecipientType.Everyone) : base(contract)
        {
            this.rtype = rtype;
            foreach(KeyValuePair<User,string> pair in contract.getRecipients(rtype))
            {
                autoSend( pair.Key, pair.Value);
            }
        }
        /// <summary>
        /// Sends a custom email to one user
        /// </summary>
        /// <param name="person">the person it is being send to</param>
        /// <param name="role">the role of the person</param>
        private void autoSend(User person, string role = "Dispatcher")
        {
            String name = "Anonymous";
            String nameContract = "Vertrag";
            try{
                name = person.name +" "+ person.surname;
            } catch (Exception e) { }
            try {
                nameContract = contract.titel;
            } catch (Exception e) { }
            String link = this.link+"/edit/"+contract.intID;
            Add(person);
            setText(name, nameContract, link, role);
            send();
            clear();
        }
        /// <summary>
        /// Sets the text and the subject of the mail, it fills the parameters into the blanks
        /// </summary>
        /// <param name="name">name of the user</param>
        /// <param name="nameContract">name of the contract</param>
        /// <param name="link">link to the contract</param>
        /// <param name="role">role of the user</param>
        private void setText(String name, String nameContract, String link, String role)
        {
            String custom1 = rtype == Contract.RecipientType.Everyone ? "Das ist die letzte Erinnerung, sie wurde an alle Betreffende versendet." : "Das ist die erste Erinnerung, sie wurde nur an die Dispatcher gesendet.";

            String custom2 = ".";
            
            if (contract.noticePeriod.HasValue && contract.endDate.HasValue)
            {
                DateTime noticeDate = contract.startDate.Value.AddDays(contract.noticePeriod.Value);
                custom2 = String.Format(", die Kündigungsfrist des Vertrages ist der {0} und das Enddatum ist der {1}.", noticeDate.ToString("dd.MM.yyyy"), contract.endDate.Value.ToString("dd.MM.yyyy"));
            } else if (contract.noticePeriod.HasValue)
            {
                DateTime noticeDate = contract.startDate.Value.AddDays(contract.noticePeriod.Value);
                custom2 = String.Format(", die Küngigungsfrist des Vertrages ist der {0}.", noticeDate.ToString("dd.MM.yyyy"));
            } else if (contract.endDate.HasValue)
            {
                custom2 = String.Format(", das Enddatum ist der {0}.", contract.endDate.Value.ToString("dd.MM.yyyy"));
            }
           
            String content =
@"Sehr geehrte Frau/geehrter Herr {0},

Sie wurden für den Vertrag '{1}' als {3} zugeteilt{4}
{5}
Bitte überprüfen Sie, ob dies gewollt ist oder nicht und handeln Sie dann entsprechend.
Sie können den Vertrag über folgenden Link aufrufen:
{2}
";

            setText(String.Format(content, name, nameContract, link, role,custom2,custom1));
            setSubject("Terminerinnerung");
        }
    }
}