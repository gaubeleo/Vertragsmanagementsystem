using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;


namespace Vertragsmanagement.Models.Manager
{
    /// <summary>
    /// An object of this class is being created by the task
    /// </summary>
    public class Manager
    {

        private ContractDBContext db = new ContractDBContext();
        /// <summary>
        /// examines every single contract
        /// </summary>
        public Manager()
        {
            try
            {
                foreach (Contract contract in db.Contracts.ToList())
                {
                    examine(contract);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// examines a contract escalations and dates
        /// </summary>
        /// <param name="contract">the contract that is being examined</param>
        private void examine(Contract contract)
        {
            if (contract.state == State.gelöscht)
                return;
            if (contract.isEscalating())
            {
                contract.reminded = true;
                db.SaveChanges();
                if (db.Tasks.Where(m => m.contractID == contract.intID).Where(m => m.describtion == Task.Description.Escalation).Count() == 0)
                    return;
                new EscalationMail(contract, Contract.RecipientType.Everyone); try
                {

                    db.Tasks.Where(m => m.contractID == contract.intID).Where(m => m.describtion == Task.Description.Escalation).First().AddUsers(contract.getRecipients(Contract.RecipientType.Everyone).Keys.ToList());

                }
                catch (Exception)
                {

                }
                
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1} in: {2}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage,
                                                    contract.titel);
                        }
                    }
                }
            }

            if (contract.isEnding())
            {
                db.SaveChanges();
            }
            //if (contract.isNoticing() && false) {  //ACHTUNG:   wird nicht mehr verwendet
            //    new NoticeMail(contract,dispatcher,role);
            //    try
            //    {
            //        dispatcher.AddTask(contract.intID, "Kündigungsfrist");
            //    }
            //    catch (Exception e) {  }
            //    try
            //    {
            //        db.SaveChanges();
            //    }
            //    catch (DbEntityValidationException dbEx)
            //    {
            //        foreach (var validationErrors in dbEx.EntityValidationErrors)
            //        {
            //            foreach (var validationError in validationErrors.ValidationErrors)
            //            {
            //                Trace.TraceInformation("Property: {0} Error: {1} in: {2}",
            //                                        validationError.PropertyName,
            //                                        validationError.ErrorMessage,
            //                                        contract.titel);
            //            }
            //        }
            //    }
            //}
            if (contract.isReminding())
            {

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1} in: {2}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage,
                                                    contract.titel);
                        }
                    }
                }
                new EscalationMail(contract,Contract.RecipientType.dispatchers);
                db.Tasks.Add(new Task(contract.intID, Task.Description.Escalation, contract.endDate.Value, contract.getRecipients(Contract.RecipientType.dispatchers).Keys.ToList()));
                
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1} in: {2}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage,
                                                    contract.titel);
                        }
                    }
                }
            }
        }
    }
}