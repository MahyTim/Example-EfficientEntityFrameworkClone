using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace EFCloner
{
    public class MyDatabase : DbContext
    {
        public DbSet<ProcessDefinition> ProcessDefinitions { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        public MyDatabase CreateNested()
        {
            var nested = new MyDatabase(Database.Connection);
            if (nested.Database.CurrentTransaction == null)
                nested.Database.UseTransaction(Database.CurrentTransaction?.UnderlyingTransaction);
            return nested;
        }

        public MyDatabase()
            : base()
        {
        }

        private MyDatabase(DbConnection parent) : base(parent, false)
        {
        }

        public IQueryable<IProcessDefinitionDependent> AllProcessDefinitionDependents(Guid processDefinitionId)
        {
            var processDefinitionDependents = GetType()
                .GetProperties()
                .Select(p => p.GetValue(this))
                .OfType<IQueryable<IProcessDefinitionDependent>>().OrderBy(x => Guid.NewGuid());

            var entities = processDefinitionDependents
                .SelectMany(pd => pd.Where("ProcessDefinition.Id = @0", processDefinitionId))
                .AsQueryable();

            return entities;
        }

    }
}