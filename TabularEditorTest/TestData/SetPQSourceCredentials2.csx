var Con = Model.DataSources[0] as StructuredDataSource; // Variable to store the first Connection

Con.Name = Environment.GetEnvironmentVariable("Name"); // Update DataSource Display Name
Con.AuthenticationKind = "UsernamePassword";
Con.Username = Environment.GetEnvironmentVariable("SQLUserName"); // Update the SQL Username
Con.Password = Environment.GetEnvironmentVariable("LOADER-PWD"); // Update the SQL Password from KeyVault
Con.Database = Environment.GetEnvironmentVariable("SQLDatabaseName"); // Update the SQL Database Name
Con.Server = Environment.GetEnvironmentVariable("SQLServerName"); // Update the SQL Username
Con.Credential["path"] = Environment.GetEnvironmentVariable("ServerPath"); // Updates connection