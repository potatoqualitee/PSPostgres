using Npgsql;
using System.Data;
using System.Collections;
using System.Management.Automation;
using System.Collections.Specialized;

namespace PSPostgres
{
    [Cmdlet(VerbsLifecycle.Invoke, "PostgresQuery")]
    [CmdletBinding(DefaultParameterSetName = "Default")]
    public class InvokePostgresQuery : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public NpgsqlConnection Connection { get; set; } = null!;

        [Parameter(Mandatory = true)]
        public string Query { get; set; }

        [Parameter()]
        public SwitchParameter AsDataTable { get; set; }

        [Parameter()]
        public SwitchParameter AsHashTable { get; set; }

        [Parameter()]
        public Hashtable Parameters { get; set; }

        // maybe should be https://learn.microsoft.com/en-us/azure/cosmos-db/postgresql/quickstart-app-stacks-csharp

        protected override void ProcessRecord()
        {
            var factory = NpgsqlFactory.Instance;
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }

                using var command = new NpgsqlCommand(Query, Connection);

                if (Parameters != null)
                {
                    foreach (DictionaryEntry parameter in Parameters)
                    {
                        var dbParameter = factory.CreateParameter();
                        dbParameter.ParameterName = parameter.Key.ToString();
                        dbParameter.Value = parameter.Value;
                        command.Parameters.Add(dbParameter);
                    }

                }

                var adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (AsDataTable)
                {
                    WriteObject(dataTable);
                    return;
                }
                
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (AsHashTable)
                    {
                        var output = new OrderedDictionary();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (dataRow[column] is DBNull)
                            {
                                output.Add(column.ColumnName, null);
                            }
                            else
                            {
                                output.Add(column.ColumnName, dataRow[column]);
                            }
                        }
                        WriteObject(output);
                    } else
                    {
                        var output = new PSObject();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (dataRow[column] is DBNull)
                            {
                                output.Properties.Add(new PSNoteProperty(column.ColumnName, null));
                            }
                            else
                            {
                                output.Properties.Add(new PSNoteProperty(column.ColumnName, dataRow[column]));
                            }
                        }
                        WriteObject(new PSObject(output));
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}