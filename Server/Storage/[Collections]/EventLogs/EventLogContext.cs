namespace Server.Storage
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

        public EventLogContext(string dbConnection)
            : base(dbConnection)
        {
        }

        #endregion

        #region Methods

        public void AddEventLogToDt(string message)
        {
            var eventLog = new EventLog
            {
                Timestamp = DateTime.Now,
                Message = message
            };
            EventLogs.Add(eventLog);
            SaveChanges();
        }

        public DataTable GetEventLogs()
        {
            return EventLogs.ToDataTable();
        }

        #endregion
    }

    public static class DataTableExtensions
    {
        #region Methods

        public static DataTable ToDataTable(this DbSet<EventLog> data)
        {
            IOrderedQueryable<EventLog> orderedData = data.OrderByDescending(eventLog => eventLog.Id);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(EventLog));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties.Cast<PropertyDescriptor>().Where(prop => prop.Name != "Id"))
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (EventLog item in orderedData)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties.Cast<PropertyDescriptor>().Where(prop => prop.Name != "Id"))
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }
            return table;
        }

        #endregion
    }
}
