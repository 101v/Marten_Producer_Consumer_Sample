using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using insight123.Messaging.Contract;

namespace Messages
{
    public class EmployeeCreated : Event
    {
        public string Name { get; set; }
    }

    public class ChangeEmployeeName : Event
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
    }
}
