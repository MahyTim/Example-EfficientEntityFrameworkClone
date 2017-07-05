using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCloner
{
    class Program
    {
        static void Main(string[] args)
        {

            Guid processDefinitionId = Guid.Empty;

            using (var db = new MyDatabase())
            {
                RemoveAll(db);

                var processDefinition = db.ProcessDefinitions.Add(new ProcessDefinition()
                {
                    Id = Guid.NewGuid()
                });
                AddTestData(db, processDefinition);
                processDefinitionId = processDefinition.Id;
                Assert(db, db.ProcessDefinitions.Find(processDefinitionId), 1);
            }

            using (var db = new MyDatabase())
            {
                Assert(db, db.ProcessDefinitions.Find(processDefinitionId), 1);
                var newPd = db.Clone(db.ProcessDefinitions.Find(processDefinitionId), true);
                Assert(db, db.ProcessDefinitions.Find(processDefinitionId), 2);
                Assert(db, newPd, 2);
            }

            using (var db = new MyDatabase())
            {
                var newPd = db.Clone(db.ProcessDefinitions.Find(processDefinitionId), true);
                Assert(db, db.ProcessDefinitions.Find(processDefinitionId), 3);
                Assert(db, newPd, 3);
            }

            using (var db = new MyDatabase())
            {
                Print(db);
            }
        }

        private static void Print(MyDatabase db)
        {
            foreach (var processDefinition in db.ProcessDefinitions)
            {
                Console.WriteLine($"PD {processDefinition.Id}");
                foreach (var customer in db.Customers.Where(z => z.ProcessDefinition.Id == processDefinition.Id))
                {
                    Console.WriteLine($"    Customer {customer.Name}");
                    foreach (var order in db.Orders.Where(z => z.Customer.Id == customer.Id))
                    {
                        Console.WriteLine($"        Order {order.Name}");
                    }
                }
            }
        }

        private static void Assert(MyDatabase db, ProcessDefinition find, int expectedCountPds)
        {
            if (db.ProcessDefinitions.Count() != expectedCountPds)
                throw new NotSupportedException("PD count wrong");
            if (db.Customers.Count() != expectedCountPds * 2)
                throw new NotSupportedException("Customers count");
            if (db.Orders.Count() != expectedCountPds * 5)
                throw new NotSupportedException("Customers count");
            if (db.Orders.Select(z => z.Customer).Distinct().Count() != 2 * expectedCountPds)
                throw new NotSupportedException("Wrong coupling to customer");

            foreach (var pd in db.ProcessDefinitions)
            {
                if (db.AllProcessDefinitionDependents(pd.Id).Count() != 7)
                    throw new NotSupportedException("Wrong coupling to pd");
            }
        }

        private static void AddTestData(MyDatabase db, ProcessDefinition processDefinition)
        {
            db.Customers.Add(new Customer()
            {
                ProcessDefinition = processDefinition,
                Name = "Customer1",
                Id = Guid.NewGuid(),
                Orders = new List<Order>()
                {
                    new Order()
                    {
                        ProcessDefinition = processDefinition,
                        Id = Guid.NewGuid(),
                        Name = "Customer1Order1"
                    },
                    new Order()
                    {
                        ProcessDefinition = processDefinition,
                        Id = Guid.NewGuid(),
                        Name = "Customer1Order2"
                    },
                }
            });
            db.Customers.Add(new Customer()
            {
                ProcessDefinition = processDefinition,
                Name = "Customer2",
                Id = Guid.NewGuid(),
                Orders = new List<Order>()
                {
                    new Order()
                    {
                        ProcessDefinition = processDefinition,
                        Id = Guid.NewGuid(),
                        Name = "Customer2Order2"
                    },
                    new Order()
                    {
                        ProcessDefinition = processDefinition,
                        Id = Guid.NewGuid(),
                        Name = "Customer2Order2"
                    },
                    new Order()
                    {
                        ProcessDefinition = processDefinition,
                        Id = Guid.NewGuid(),
                        Name = "Customer2Order3"
                    },
                }
            });

            db.SaveChanges();
        }

        private static void RemoveAll(MyDatabase db)
        {
            foreach (var dbOrder in db.Orders)
            {
                db.Orders.Remove(dbOrder);
            }
            foreach (var dbCustomer in db.Customers)
            {
                db.Customers.Remove(dbCustomer);
            }
            foreach (var dbProcessDefinition in db.ProcessDefinitions)
            {
                db.ProcessDefinitions.Remove(dbProcessDefinition);
            }
            db.SaveChanges();
        }
    }
}
