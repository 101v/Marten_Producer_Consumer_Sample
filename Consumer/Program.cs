using System;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Events.Projections.Async;
using Marten.Storage;
using Messages;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = DocumentStore.For(_ =>
            {
                _.Connection("Server=127.0.0.1;Port=5432;Database=testdb;User Id=postgres;Password=123mrp.NET;");
                _.Events.DatabaseSchemaName = "event_store";
                _.Events.StreamIdentity = StreamIdentity.AsString;
                _.Events.UseAppendEventForUpdateLock = true;
                _.Events.AsyncProjections.Add<EventProcessor>();
            });

            IProjection[] listOfProjection = {
                new EventProcessor()
            };

            using (var daemon = store.BuildProjectionDaemon(projections: listOfProjection))
            {
                daemon.StartAll();
                Console.ReadLine();
                daemon.StopAll();
            }
        }
    }


    public class EventProcessor : IProjection
    {
        public void Apply(IDocumentSession session, EventPage page)
        {
            foreach (var @event in page.Events)
            {
                Console.WriteLine(@event);
            }
        }

        public Task ApplyAsync(IDocumentSession session, EventPage page, CancellationToken token)
        {
            foreach (var @event in page.Events)
            {
                Console.WriteLine(@event);
            }

            return Task.FromResult<object>(null);
        }

        public void EnsureStorageExists(ITenant tenant)
        {
        }

        public Type[] Consumes { get; } = new Type[]
        {
            typeof(EmployeeCreated), typeof(ChangeEmployeeName)
        };

        public AsyncOptions AsyncOptions { get; } = new AsyncOptions();
    }
}
