using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using insight123.Common;
using Marten;
using Marten.Events;
using Messages;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = DocumentStore.For(_ =>
            {
                _.Connection("Server=127.0.0.1;Port=5432;Database=testdb;User Id=postgres;Password=123mrp.NET;");
                _.PLV8Enabled = false;
                _.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                _.Events.DatabaseSchemaName = "event_store";
                _.Events.StreamIdentity = StreamIdentity.AsString;
                _.Events.UseAppendEventForUpdateLock = true;
                //_.Events.AsyncProjections.Add<EventProcessor>();
            });

            Console.WriteLine("This is PRODUCER type A to Create C to Change and  E to exit");


            while (true)
            {
                var key = Console.ReadKey().Key;

                if (key == ConsoleKey.A)
                {
                    var added = new EmployeeCreated
                    {
                        Name = "Vimal Patel",
                        Creator = new Creator { CreatorId = Guid.Empty, CreatorName = "Administrator" },
                        OccurredOn = DateTimeOffset.Now,
                        SourceId = Guid.Empty,
                        Version = 1
                    };

                    using (var session = store.OpenSession())
                    {
                        session.Events.Append("TestStream001", added);
                        session.SaveChanges();
                    }
                }
                else if (key == ConsoleKey.C)
                {
                    var changed = new ChangeEmployeeName
                    {
                        OldName = "Vimal Patel",
                        NewName = "Dharmi Patel"
                    };
                    using (var session = store.OpenSession())
                    {
                        session.Events.Append("TestStream001", changed);
                        session.SaveChanges();
                    }

                }
                else
                {
                    break;
                }
            }
        }
    }
}
