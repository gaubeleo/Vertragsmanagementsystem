using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Vertragsmanagement.Models.Manager
{
    /// <summary>
    /// contains the necessary parts for sending an email, it uses sendgrid to send an email
    /// </summary>
    public abstract class Mail
    {
        private SendGridMessage message = new SendGridMessage();

        private List<String> recipients = new List<String>();
        protected Contract contract;
        protected string link = "https://soproteam15.azurewebsites.net/contract";
        public Mail(Contract contract)
        {
            this.contract = contract;
        }

        /// <summary>
        /// Adds a recipients to the E-mail
        /// </summary>
        /// <param name="recipient">the recipient of the email</param>

        public void Add(string recipient)
        {
            //Format : @"John Doe <John.Doe@gmail.com>"
            recipients.Add(recipient);
        }

        /// <summary>
        /// clears the recipientlist, so that it is not necessary to create a new mail instance
        /// </summary>
        public void clear()
        {
            message = new SendGridMessage();
            recipients.Clear();
        }

        /// <summary>
        /// Overload of default Add function, Adds a recipient to the E-mail
        /// </summary>
        /// <param name="recipient"></param>

        public void Add(MailAddress recipient)
        {
            //Format : new MailAddress("John.Doe@gmail.com","John Doe")
            recipients.Add(recipient.ToString());
        }
        /// <summary>
        /// sets the text of the mail
        /// </summary>
        /// <param name="text">the text of the mail</param>
        protected void setText(String text)
        {
            message.Text = text;
        }

        /// <summary>
        /// sets the subject of the mail
        /// </summary>
        /// <param name="subject">the subject of the mail</param>
        protected void setSubject(String subject)
        {
            message.Subject = subject;
        }
        /// <summary>
        /// adds
        /// </summary>
        /// <param name="user"></param>
        protected virtual void Add(User user)
        {
            Add(user.getEmail());
            
        }

        /// <summary>
        /// Sends the E-mail, in order to do so, it chooses the Forwarder and enters the API-key
        /// </summary>

        public void send()
        {
            message.EnableTemplateEngine("de4f0355-5d9f-4cd5-82aa-ef22fa52d6e8");
            message.From = new MailAddress("Vertragsmanagment@azure.de", "Vertragsmanagment");
            message.AddTo(recipients);
            var transport = new Web("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");
            transport.DeliverAsync(message);


        }
    }
}
