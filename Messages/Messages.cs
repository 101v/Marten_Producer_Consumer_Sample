using System;

namespace Messages
{
    public interface IEvent
    {
        Guid SourceId { get; set; }
        int Version { get; set; }
        DateTimeOffset OccurredOn { get; set; }
        Guid CorrelationId { get; set; }
    }

    public abstract class Event : IEvent
    {
        protected Event()
        { }

        public Guid SourceId { get; set; }
        public int Version { get; set; }
        public DateTimeOffset OccurredOn { get; set; }
        public Guid CorrelationId { get; set; }
    }

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
