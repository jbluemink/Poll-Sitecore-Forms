using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace RadioPollResult.Services
{
    public class SqlPoll
    {
        public Dictionary<string, int> GetAggregatedPollValues(Guid FormDefinitionId, Guid FormFieldId)
        {
            var setting = new Sitecore.ExperienceForms.Configuration.FormsConfigurationSettings();
            var connectionsting = ConfigurationManager.ConnectionStrings[setting.ConnectionStringName].ConnectionString;
            Dictionary<string, int> result = null;

            string commandText = "SELECT Value, Count(0) " +
                                 "FROM [sitecore_forms_storage].[FieldData] fielddata, [sitecore_forms_storage].[FormEntries] formentries " +
                                 "WHERE formentries.FormDefinitionId = @FormID " +
                                 "AND formentries.Id = fielddata.FormEntryId " +
                                 "AND fielddata.FieldDefinitionId = @FieldID " +
                                 "GROUP by Value";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionsting))
                {
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.Add("@FormID", SqlDbType.UniqueIdentifier);
                    command.Parameters["@FormID"].Value = FormDefinitionId;
                    command.Parameters.Add("@FieldID", SqlDbType.UniqueIdentifier);
                    command.Parameters["@FieldID"].Value = FormFieldId;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    result = this.ParseResults(reader);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Forms database communication error.", ex, (object)this);
            }

            return result;

        }

        protected Dictionary<string, int> ParseResults(SqlDataReader reader)
        {
            var pollvalues = new  Dictionary<string, int>();
            while (reader.Read())
            {
                string str = reader.GetString(0);
                int count =  reader.GetInt32(1);
                if (!string.IsNullOrEmpty(str))
                    pollvalues.Add(str,count);
            }
            return pollvalues;
        }

    }
}