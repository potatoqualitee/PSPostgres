using Npgsql;
using System.Management.Automation;

namespace PSPostgres
{
    [Cmdlet(VerbsCommunications.Connect, "Postgres")]
    [OutputType(typeof(NpgsqlConnection))]
    [CmdletBinding(DefaultParameterSetName = "Default")]
    public class ConnectPostgres : PSCmdlet
    {
        [Parameter(ParameterSetName = "Connectionstring", Mandatory = true)]
        public string ConnectionString { get; set; }
        
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public PSCredential Credential { get; set; }

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string Server { get; set; } = "localhost";

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public int Port { get; set; } = 5432;

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string Database { get; set; } = "postgres";

        protected override void ProcessRecord()
        {
            var factory = NpgsqlFactory.Instance;
            var pgStringBuilder = factory.CreateConnectionStringBuilder();
 
            if (ConnectionString != null)
            {
                pgStringBuilder.ConnectionString = ConnectionString;
            }
            else
            {
                if (Credential != null)
                {
                    pgStringBuilder.Add("Username", Credential.GetNetworkCredential().UserName);
                    pgStringBuilder.Add("Password", Credential.GetNetworkCredential().Password);
                }
                if (Database != null)
                {
                    pgStringBuilder.Add("Database", Database);
                }
                
                if (Server != null)
                {
                    pgStringBuilder.Add("Host", Host);
                }
                if (Database != null)
                {
                    pgStringBuilder.Add("Database", Database);
                }

                pgStringBuilder.Add("Port", Port);
            }

            var connection = factory.CreateConnection();
            
            try
            {
                connection.ConnectionString = pgStringBuilder.ToString();
                connection.Open();
                WriteObject(connection);
            }
            catch
            {
                throw;
            }
        }
    }
}