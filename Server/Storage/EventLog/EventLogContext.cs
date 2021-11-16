namespace Server.Storage.EventLog
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;

    public class EventLogContext : DbContext
    {
        #region Properties

        public DbSet<EventLog> EventLogs { get; set; }

        #endregion

        #region Constructors

        public EventLogContext()
            : base("DbConnection")
        {
        }

        #endregion

        #region Methods

        public void ConnectionEventLog(string message)
        {
            var eventLog = new EventLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Message = message
            };
            EventLogs.Add(eventLog);
            SaveChanges();
        }

        public void ClearEventLogsDt()
        {
            Database.ExecuteSqlCommand("TRUNCATE TABLE [EventLogs]");
        }

        #endregion
    }

    public static class DataTableExtensions
    {
        #region Methods

        public static DataTable ToDataTable<T>(this DbSet<T> data)
            where T : class
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties.Cast<PropertyDescriptor>().Where(prop => prop.Name != "Id"))
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties.Cast<PropertyDescriptor>().Where(prop => prop.Name != "Id"))
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            OrderedEnumerableRowCollection<DataRow> orderedRows = from row in table.AsEnumerable()
                                                                  orderby row.Field<DateTime>("Timestamp") descending
                                                                  select row;
            DataTable orderedTable = orderedRows.CopyToDataTable();
            return orderedTable;
        }

        #endregion
    }
}
