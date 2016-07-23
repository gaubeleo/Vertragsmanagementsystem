using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vertragsmanagement.Models.Manager
{
    class NoticeMail : Mail
    {
        public NoticeMail(Contract contract,User person ,string role = "Dispatcher") : base(contract)
        {
            foreach (User dispatcher in contract.observingDepartment.dispatchers.Where((User) => User != null).ToList())
            {
                autoSend(contract, dispatcher);
            }
            foreach (User coordinator in contract.observingDepartment.mandant.coordinators.ToList())
            {
                autoSend(contract, coordinator, "Koordinator");
            }
            autoSend(contract, contract.personInCharge, "Sachlicher Verantwortlicher");
            clear();
        }

        public void autoSend(Contract contract, User person, String role = "Dispatcher")
        {
            String name;
            String contractName;           
            
            name = person.FullName;
            contractName = contract.titel;
            
            Add(person);
            int deadline = (int)contract.startDate.Value.Add(new TimeSpan(contract.noticePeriod.Value, 0, 0, 0)).Subtract(DateTime.Now).TotalDays;
            setText(name, contractName, "www.google.de", deadline);
        }

        public void setText(String name, String nameContract, String link, int deadline)
        {
            String content =
@"Sehr geehrte Frau/geehrter Herr {0},

Sie sind Dispatcher eines Vertrages mit dem Namen {1}, für diesen Vertrag läuft in {3} Tagen die Kündigungsfrist aus.
Sie können den Vertrag über folgenden Link aufrufen:
{2}


Wir freuen uns, dass wir Ihnen helfen konnten!";

            setText(String.Format(content, name, nameContract, link, deadline));
            setSubject("Kündigungsfrist läuft aus");
        }
    }
}
