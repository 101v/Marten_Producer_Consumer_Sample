using System;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Events.Projections.Async;
using Marten.Events.Projections.Async.ErrorHandling;
using Marten.Storage;
using Messages;
using Topshelf;
using Topshelf.Logging;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.UseNLog();

                var store = DocumentStore.For(_ =>
                {
                    _.Connection("Server=127.0.0.1;Port=5432;Database=testdb;User Id=postgres;Password=123mrp.NET;");
                    _.Events.DatabaseSchemaName = "event_store";
                    _.Events.StreamIdentity = StreamIdentity.AsString;
                    _.Events.UseAppendEventForUpdateLock = true;
                });

                IProjection[] projections = {
                    new EventProcessor(0, store),
                    new EventProcessor01(1, store),
                    new EventProcessor02(2, store),
                    new EventProcessor03(3, store),
                    new EventProcessor04(4, store),
                    new EventProcessor05(5, store),
                    new EventProcessor06(6, store),
                    new EventProcessor07(7, store),
                    new EventProcessor08(8, store),
                    new EventProcessor09(9, store),
                    new EventProcessor10(10, store),
                    new BadProcessor(11, store)
                };

                var logWriter = HostLogger.Get("Consumer");

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.EnableShutdown();
                x.EnableServiceRecovery(c => c.RestartService(1));
                x.Service<EventSubscription>(s =>
                {
                    s.ConstructUsing(name => new EventSubscription(store, logWriter, new DaemonLogger(logWriter)));
                    s.WhenStarted((tc, hc) => tc.Start(hc, projections));
                    s.WhenStopped(tc => tc.Stop());
                });

                x.SetServiceName("EventDispatcherServiceName");
                x.SetDisplayName("EventDispatcherServiceDisplayName");
                x.SetDescription("EventDispatcherServiceDescription");
            });
        }
    }

    public class EventSubscription
    {
        private readonly IDocumentStore documentStore;
        private IDaemon daemon;
        private readonly LogWriter logger;
        private readonly IDaemonLogger daemonLogger;
        private HostControl hostControl;

        public EventSubscription(IDocumentStore documentStore, LogWriter logger, IDaemonLogger daemonLogger)
        {
            this.documentStore = documentStore;
            this.logger = logger;
            this.daemonLogger = daemonLogger;
        }

        public bool Start(HostControl hc, IProjection[] projections)
        {
            hostControl = hc;

            var settings = new DaemonSettings
            {
                FetchingCooldown = TimeSpan.FromMilliseconds(500),
                LeadingEdgeBuffer = TimeSpan.FromMilliseconds(100)
            };

            settings.ExceptionHandling
                .OnException<Exception>()
                .Retry(3, TimeSpan.FromSeconds(10))
                .AfterMaxAttempts = new StopAll(x =>
                {
                    logger.Error(x);
                    hc.Stop();
                });

            daemon = documentStore.BuildProjectionDaemon(logger: daemonLogger, settings: settings, projections: projections);

            logger.Info("Starting service...");
            daemon.StartAll();

            return true;
        }

        public void Stop()
        {
            try
            {
                logger.Info("Stopping service...");
                //daemon?.StopAll();
                logger.Info("Service stopped.");
            }
            catch (Exception exception)
            {
                //Thrown by the subscription where it could not stop in the given time, Log this exception
                logger.Error(exception);
            }
        }
    }

    public class EventProcessor : IProjection
    {
        private readonly int id;
        private readonly IDocumentStore store;

        public EventProcessor(int id, IDocumentStore store, Type[] events = null)
        {
            this.id = id;
            this.store = store;

            Consumes = events ?? new[] { typeof(EmployeeCreated), typeof(EmployeeNameChanged) };

        }
        public void Apply(IDocumentSession session, EventPage page)
        {
            throw new NotImplementedException();
        }

        public Task ApplyAsync(IDocumentSession session, EventPage page, CancellationToken token)
        {
            foreach (var @event in page.Events)
            {
                if (@event.Data != null)
                {
                    Console.WriteLine(@event);
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    if (@event.Data is FailedProjection fp)
                    {
                        var failed = new FailedProjectionCaused()
                        {
                            Reason = fp.Reason
                        };

                        using (var newSession = store.OpenSession())
                        {
                            newSession.Events.Append("DeadMessageStream001", failed);
                            newSession.SaveChanges();
                        }

                        throw new Exception("Dummy");
                    }
                }
            }

            return Task.FromResult<object>(null);
        }

        public void EnsureStorageExists(ITenant tenant)
        {
        }

        public Type[] Consumes { get; }

        public AsyncOptions AsyncOptions { get; } = new AsyncOptions {PageSize = 10};
    }

    public class EventProcessor01 : EventProcessor
    {
        public EventProcessor01(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor02 : EventProcessor
    {
        public EventProcessor02(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor03 : EventProcessor
    {
        public EventProcessor03(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor04 : EventProcessor
    {
        public EventProcessor04(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor05 : EventProcessor
    {
        public EventProcessor05(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor06 : EventProcessor
    {
        public EventProcessor06(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor07 : EventProcessor
    {
        public EventProcessor07(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor08 : EventProcessor
    {
        public EventProcessor08(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor09 : EventProcessor
    {
        public EventProcessor09(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class EventProcessor10 : EventProcessor
    {
        public EventProcessor10(int id, IDocumentStore store) : base(id, store)
        { }
    }

    public class BadProcessor : EventProcessor
    {
        public BadProcessor(int id, IDocumentStore store)
            : base(id,
                store,
                new[] { typeof(EmployeeCreated), typeof(EmployeeNameChanged), typeof(FailedProjection) })
        {
        }
    }
}
