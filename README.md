# PSPostgres
 
Simple PowerShell binary wrapper for Npgsql, somewhat based on [cbsch-pgsql](https://github.com/cbsch/cbsch-pgsql).

For more advanced functionality, check out [dbops](https://github.com/dataplat/dbops).

# Examples

```powershell
# Connect to a Postgres database and execute a statement that creates a table.

$conn = Connect-Postgres -ConnectionString $env:PGSQLConn
Invoke-PostgresQuery -Connection $conn -Query @"
CREATE TABLE test(
    id int,
    guid uuid,
    date timestamp with time zone,
    smallint integer,
    bigint bigint,
    double double precision
)
"@

# Connect to a Postgres database and execute a statement that safely inserts a row using a parameterized query.

$conn = Connect-Postgres -ConnectionString $env:PGSQLConn
Invoke-PostgresQuery -Connection $conn -Query @"
INSERT INTO test(id, guid, date, smallint, bigint, double)
VALUES(@id, @guid, @date, @smallint, @bigint, @double)
"@ -Parameters @{
    "@id" = 1
    "@guid" = [Guid]::NewGuid()
    "@date" = [DateTime]::Now
    "@smallint" = 42
    "@bigint" = [Int64]::MaxValue
    "@double" = 0.23
}

# Connects to a Postgres database and executes a simple query.

$conn = Connect-Postgres -ConnectionString $env:PGSQLConn
Invoke-PostgresQuery -Connection $conn -Query "SELECT * FROM test"
```