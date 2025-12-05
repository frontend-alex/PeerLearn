# Remote SQL Server Setup for Raspberry Pi

If you already have SQL Server running on another server, you can use it with your Raspberry Pi deployment. No code changes needed!

## Prerequisites

- SQL Server running on Windows/Linux server (accessible from network)
- SQL Server Authentication enabled (or Windows Authentication if on same domain)
- Network connectivity between Raspberry Pi and SQL Server

## Step 1: Configure SQL Server for Remote Access

### On SQL Server Machine:

1. **Enable TCP/IP Protocol:**
   - Open **SQL Server Configuration Manager**
   - Navigate to **SQL Server Network Configuration** → **Protocols for [Instance Name]**
   - Right-click **TCP/IP** → **Enable**
   - Restart SQL Server service

2. **Configure SQL Server Port:**
   - Right-click **TCP/IP** → **Properties**
   - Go to **IP Addresses** tab
   - Scroll to **IPAll** section
   - Set **TCP Port** (default is 1433)
   - Clear **TCP Dynamic Ports** if set
   - Click **OK** and restart SQL Server

3. **Enable SQL Server Authentication (if using username/password):**
   - Open **SQL Server Management Studio**
   - Right-click server → **Properties**
   - Go to **Security** page
   - Select **SQL Server and Windows Authentication mode**
   - Click **OK** and restart SQL Server

4. **Create/Verify Database User:**
   ```sql
   -- If user doesn't exist, create it
   CREATE LOGIN peerlearnuser WITH PASSWORD = 'YourSecurePassword123!';
   
   -- Grant access to database
   USE PeerLearnDb;
   CREATE USER peerlearnuser FOR LOGIN peerlearnuser;
   ALTER ROLE db_owner ADD MEMBER peerlearnuser;
   ```

5. **Configure Windows Firewall:**
   - Open **Windows Firewall with Advanced Security**
   - Create **Inbound Rule**
   - Rule Type: **Port**
   - Protocol: **TCP**
   - Port: **1433** (or your custom port)
   - Action: **Allow the connection**
   - Apply to: **Domain, Private, Public** (or just Private for local network)

## Step 2: Test Connection from Raspberry Pi

On your Raspberry Pi:

```bash
# Install SQL Server command-line tools (optional, for testing)
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
curl https://packages.microsoft.com/config/debian/12/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list
sudo apt update
sudo ACCEPT_EULA=Y apt install -y mssql-tools unixodbc-dev

# Add to PATH
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc

# Test connection
sqlcmd -S <your-sql-server-ip> -U peerlearnuser -P 'YourSecurePassword123!' -Q "SELECT @@VERSION"
```

If connection succeeds, you're good to go!

## Step 3: Update Connection String

On your Raspberry Pi, in `~/peerlearn-api/.env`:

```bash
# Format for SQL Server Authentication
CONNECTION_STRING=Server=<your-sql-server-ip>,1433;Database=PeerLearnDb;User Id=peerlearnuser;Password=YourSecurePassword123!;TrustServerCertificate=true;MultipleActiveResultSets=true;

# Format for Windows Authentication (if on same domain)
CONNECTION_STRING=Server=<your-sql-server-ip>,1433;Database=PeerLearnDb;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true;

# If using named instance
CONNECTION_STRING=Server=<your-sql-server-ip>\SQLEXPRESS,1433;Database=PeerLearnDb;User Id=peerlearnuser;Password=YourSecurePassword123!;TrustServerCertificate=true;
```

**Important Parameters:**
- `Server`: IP address or hostname of SQL Server
- `Database`: Your database name
- `User Id` / `Password`: SQL Server credentials
- `TrustServerCertificate=true`: Required for encrypted connections without certificate validation
- `MultipleActiveResultSets=true`: Allows multiple queries on same connection

## Step 4: Test from .NET Application

After deploying your backend, check the logs:

```bash
sudo journalctl -u peerlearn-api -f
```

Look for connection string output and any database errors.

## Security Best Practices

1. **Use Strong Passwords**: Generate secure passwords for database users
2. **Limit Network Access**: Only allow connections from your Raspberry Pi IP
3. **Use SSL/TLS**: Configure SQL Server to require encrypted connections
4. **Regular Updates**: Keep SQL Server updated with latest security patches
5. **Firewall Rules**: Restrict SQL Server port to only necessary IPs
6. **Separate User**: Use dedicated user for application (not sa)

## Troubleshooting

### Connection Timeout
- Check firewall rules on SQL Server machine
- Verify SQL Server is listening on correct port: `netstat -an | findstr 1433`
- Check SQL Server Browser service is running (for named instances)

### Authentication Failed
- Verify SQL Server Authentication is enabled
- Check username/password are correct
- Ensure user has access to the database

### Network Issues
- Ping SQL Server from Raspberry Pi: `ping <sql-server-ip>`
- Test port connectivity: `telnet <sql-server-ip> 1433` or `nc -zv <sql-server-ip> 1433`
- Check if SQL Server is accessible from network

### SSL/Certificate Errors
- Add `TrustServerCertificate=true` to connection string (for testing)
- For production, configure proper SSL certificates

## Performance Tips

1. **Connection Pooling**: EF Core handles this automatically
2. **Network Latency**: Consider connection timeout settings if network is slow
3. **Monitoring**: Use SQL Server Profiler to monitor queries from Pi
4. **Indexes**: Ensure proper indexes exist for your queries

## Alternative: SQL Server on Linux

If your remote server is Linux, you can also use SQL Server on Linux:
- Same connection string format
- Same configuration steps
- Better performance on Linux servers


