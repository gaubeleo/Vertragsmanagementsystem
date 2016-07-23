using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models.Manager
{
    /// <summary>
    /// a mail that will send a text to a person who has become a dispatcher of a contract
    /// </summary>
    public class DispatcherMail : Mail
    {
        /// <summary>
        /// Executes the autoSend function for each user this is pertaining to
        /// </summary>
        /// <param name="contract">The contract this email pertains to</param>
        public DispatcherMail(Contract contract, ContractDBContext db) : base(contract)
        {
            for (int i = 0; i < contract.getRecipients(Contract.RecipientType.dispatchers).Count; i++)
            {
                autoSend(contract.getRecipients(Contract.RecipientType.dispatchers).ElementAt(i).Key, db);
            }
        }
        /// <summary>
        /// Sends a custom email to one user
        /// </summary>
        /// <param name="dispatcher">user it is send to, by default a dispatcher</param>
        public void autoSend(User dispatcher, ContractDBContext db)
        {
            String link = this.link + "/edit/" + contract.intID;
            setText(dispatcher.name+" "+dispatcher.surname, contract.titel, link, contract.isCompleted(db));
            Add(dispatcher);
            send();
            clear();
        }
        /// <summary>
        /// Sets the text and the subject of the mail, it fills the parameters into the blanks
        /// </summary>
        /// <param name="nameAdressat">Name of Person</param>
        /// <param name="nameVertrag">Name of Contract</param>
        /// <param name="link">Link to contract if online</param>
        /// <param name="isComplete">true if contract complete, else false</param>
        public void setText(String nameAdressat, String nameVertrag, String link, bool isComplete = false)
        {
            String descriptionOfState = isComplete ? "Die wichtigsten Felder wurden jedoch schon eigetragen." : "Es fehlen noch wichtige Einträge des Vertages, dadurch hat er noch den Status unvollständig.";
            String content =
@"Sehr geehrte Frau/geehrter Herr {0},

Sie wurden für den Vertrag mit dem Titel '{1}' als Dispatcher zugeteilt, bitte kontrollieren und vervollständigen Sie die Daten zu dem Vetrag.
{3}
Außerdem bedeutet es, dass Sie ab sofort E-Mails, bezüglich der Fristen und Termine des Vertrages, erhalten.
Sie können den Vertrag über folgenden Link aufrufen:
{2}
";

            setText(String.Format(content, nameAdressat, nameVertrag, link,descriptionOfState));
            setSubject("Sie wurden zugewiesen!");
        }
    }
}