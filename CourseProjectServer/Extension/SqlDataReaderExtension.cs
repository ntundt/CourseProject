using System.Data.SqlClient;

namespace CourseProjectServer.Extension
{
    public static class SqlDataReaderExtension
    {
        public static T GetFieldValue<T>(this SqlDataReader reader, string fieldName, T defaultvalue = default(T))
        {
            try
            {
                var value = reader[fieldName];
                if (value == DBNull.Value || value == null)
                    return defaultvalue;
                return (T)value;
            }
            catch (Exception)
            {
                Console.WriteLine($"Can not get the value of the field {fieldName}");
                //SimpleLog.Error("Error reading databasefield " + fieldName + "| ", e);
            }
            return defaultvalue;
        }
    }
}
