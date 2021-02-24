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

    public class EmployeeNameChanged : Event
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
    }

    public class FailedProjection : Event
    {
        public string Reason { get; set; }
    }

    public class FailedProjectionCaused : Event
    {
        public string Reason { get; set; }
    }
}
