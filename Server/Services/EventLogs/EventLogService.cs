namespace Server.Services
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;

    using Storage;

    public class EventLogService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Constructors

        public EventLogService(InternalStorage storage)
        {
            _storage = storage;
        }

        #endregion

        #region Methods

        public DataTable GetEventLogs()
        {
            return _storage.EventLogs.ToDataTable();
        }

        #endregion
    }

    public static class DataTableExtensions
    {
        #region Methods

        public static DataTable ToDataTable(this DbSet<EventLog> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(EventLog));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties.Cast<PropertyDescriptor>().Where(prop => prop.Name != "Id"))
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (EventLog item in data)
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
