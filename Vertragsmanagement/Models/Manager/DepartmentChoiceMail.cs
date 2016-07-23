using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vertragsmanagement.Models.Manager
{
    /// <summary>
    /// a mail that will send a text to a person who has the task to choose a department
    /// </summary>
    public class DepartmentChoiceMail : Mail
    {
        /// <summary>
        /// Executes the autoSend function for each user this is pertaining to
        /// </summary>
        /// <param name="contract">the contract the mail pertains to</param>
        public DepartmentChoiceMail(Contract contract) : base(contract)
        {
            foreach(KeyValuePair<User,string> pair in contract.getRecipients(Contract.RecipientType.personInCharge))
            {
                autoSend(pair.Key);
            }
        }

        /// <summary>
        /// Sends a custom email to one user
        /// </summary>
        /// <param name="user">the Recipient of the mail</param>

        private void autoSend(User user)
        {
            String link = this.link + "/edit/" + contract.intID;
            setText(user.name +" "+user.surname, contract.titel, link);
            Add(user);
            send();
            clear();
        }

        /// <summary>
        /// Function that contains the Text of the Email, it fills the parameters into the blanks of the text. It also sets the subject of the email.
        /// </summary>
        /// <param name="nameAdressat"></param>
        /// <param name="nameVertrag"></param>
        /// <param name="link"></param>
        public void setText(String nameAdressat, String nameVertrag, String link)
        {
            String content =
@"Sehr geehrte Frau/geehrter Herr {0},

Sie wurden für den Vertrag '{1}' als sachlicher Verantwortlicher zugeteilt, bitte bestimmen Sie innerhalb einer Frist die Überwachende Abteilung dieses Vertrages.
Darüber werden dann auch automatisch die Dispatcher und die Koordinatoren des Vertrages bestimmt.
Sie können den Vertrag über folgenden Link aufrufen:
{2}
";

            setText(String.Format(content, nameAdressat, nameVertrag, link));
            setSubject("Überwachende Abteilung fehlt!");
        }
    }
}