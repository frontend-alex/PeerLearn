# Deployment Guide: Raspberry Pi 4

This guide covers deploying your PeerLearn application on a Raspberry Pi 4.

## Prerequisites

### Hardware Requirements
- Raspberry Pi 4 (4GB RAM minimum, 8GB recommended)
- MicroSD card (32GB+ recommended, Class 10 or better)
- Power supply (official Raspberry Pi 5V 3A USB-C)
- Network connection (Ethernet recommended for stability)

### Software Requirements
- Raspberry Pi OS (64-bit) - Bullseye or later
- .NET 9.0 Runtime (ARM64)
- Node.js 18+ and pnpm (for building frontend)
- Nginx (reverse proxy)
- **Database Options:**
  - **Option A**: Remote SQL Server (if you already have a database server) ✅ **RECOMMENDED**
  - **Option B**: PostgreSQL (if you need to host database on Pi)

## Step 1: Initial Raspberry Pi Setup

1. **Flash Raspberry Pi OS** to microSD card using Raspberry Pi Imager
2. **Enable SSH** (can be done in Imager settings)
3. **Boot the Pi** and connect via SSH:
   ```bash
   ssh pi@<raspberry-pi-ip>
   ```

4. **Update system**:
   ```bash
   sudo apt update && sudo apt upgrade -y
   sudo reboot
   ```

## Step 2: Install .NET 9.0 Runtime

.NET 9.0 supports ARM64 on Raspberry Pi:

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET 9.0 Runtime
sudo apt update
sudo apt install -y dotnet-runtime-9.0

# Verify installation
dotnet --version
```

## Step 3: Database Setup

### Option A: Use Remote SQL Server (Recommended if you have existing database)

If you already have SQL Server running on another server, you can keep using it! No migration needed.

1. **Update connection string** in `.env` on Raspberry Pi:
   ```
   CONNECTION_STRING=Server=<your-db-server-ip>;Database=PeerLearnDb;User Id=<username>;Password=<password>;TrustServerCertificate=true;
   ```

2. **Ensure SQL Server allows remote connections:**
   - On your SQL Server machine, enable TCP/IP protocol in SQL Server Configuration Manager
   - Open port 1433 in firewall (or your custom SQL Server port)
   - Enable SQL Server Authentication (if using username/password)
   - Add firewall rule to allow Raspberry Pi IP

3. **Test connection from Raspberry Pi:**
   ```bash
   # Install SQL Server tools (optional, for testing)
   curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
   curl https://packages.microsoft.com/config/debian/12/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list
   sudo apt update
   sudo ACCEPT_EULA=Y apt install -y mssql-tools
   
   # Test connection (optional)
   /opt/mssql-tools/bin/sqlcmd -S <your-db-server-ip> -U <username> -P <password> -Q "SELECT @@VERSION"
   ```

**Advantages:**
- ✅ No code changes needed
- ✅ Keep existing database
- ✅ Better performance (dedicated database server)
- ✅ Easier backups and maintenance

**Skip to Step 5** if using remote SQL Server.

---

### Option B: Install PostgreSQL (Only if hosting database on Pi)

If you need to host the database on the Raspberry Pi itself:

```bash
# Install PostgreSQL
sudo apt install -y postgresql postgresql-contrib

# Start and enable PostgreSQL
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Create database and user
sudo -u postgres psql
```

In PostgreSQL prompt:
```sql
CREATE DATABASE peerlearndb;
CREATE USER peerlearnuser WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE peerlearndb TO peerlearnuser;
\q
```

Then follow the PostgreSQL migration guide in `app/server/Infrastructure/Persistence/SQL/PostgreSQL_Migration_Guide.md`

## Step 4: Update Backend (Only if using PostgreSQL)

**Skip this step if using remote SQL Server!**

If using PostgreSQL, you'll need to modify your backend:

1. **Update `Infrastructure.csproj`**:
   ```xml
   <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.2" />
   <!-- Remove Microsoft.EntityFrameworkCore.SqlServer -->
   ```

2. **Update `Program.cs`**:
   ```csharp
   // Change from:
   options.UseSqlServer(...)
   // To:
   options.UseNpgsql(...)
   ```

3. **Update connection string** in `.env`:
   ```
   CONNECTION_STRING=Host=localhost;Database=peerlearndb;Username=peerlearnuser;Password=your_secure_password
   ```

## Step 5: Build and Deploy Backend

**Note:** If using remote SQL Server, no code changes are needed. Just update the connection string!

1. **On your development machine**, build the backend:
   ```bash
   cd app/server/API
   dotnet publish -c Release -r linux-arm64 --self-contained false
   ```

2. **Transfer files to Raspberry Pi**:
   ```bash
   # From your dev machine
   scp -r app/server/API/bin/Release/net9.0/linux-arm64/publish pi@<raspberry-pi-ip>:~/peerlearn-api
   scp app/server/API/.env pi@<raspberry-pi-ip>:~/peerlearn-api/
   ```

3. **On Raspberry Pi**, set permissions:
   ```bash
   chmod +x ~/peerlearn-api/API
   ```

## Step 6: Build and Deploy Frontend

1. **On your development machine**, build the frontend:
   ```bash
   cd app/client
   pnpm install
   pnpm build
   ```

2. **Transfer build files**:
   ```bash
   scp -r app/client/dist pi@<raspberry-pi-ip>:~/peerlearn-frontend
   ```

## Step 7: Install and Configure Nginx

```bash
# Install Nginx
sudo apt install -y nginx

# Create Nginx configuration
sudo nano /etc/nginx/sites-available/peerlearn
```

Add this configuration:
```nginx
server {
    listen 80;
    server_name your-domain.com;  # or your Pi's IP address

    # Frontend
    location / {
        root /home/pi/peerlearn-frontend;
        try_files $uri $uri/ /index.html;
    }

    # Backend API
    location /api {
        proxy_pass http://localhost:5106;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # SignalR Hub
    location /hubs {
        proxy_pass http://localhost:5106;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

Enable the site:
```bash
sudo ln -s /etc/nginx/sites-available/peerlearn /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
sudo systemctl enable nginx
```

## Step 8: Create Systemd Service for Backend

Create a service file for auto-starting the API:

```bash
sudo nano /etc/systemd/system/peerlearn-api.service
```

Add:
```ini
[Unit]
Description=PeerLearn API
After=network.target postgresql.service

[Service]
Type=notify
User=pi
WorkingDirectory=/home/pi/peerlearn-api
ExecStart=/usr/bin/dotnet /home/pi/peerlearn-api/API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=peerlearn-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Enable and start:
```bash
sudo systemctl daemon-reload
sudo systemctl enable peerlearn-api
sudo systemctl start peerlearn-api
sudo systemctl status peerlearn-api
```

## Step 9: Update Frontend Configuration

Update `app/client/src/lib/config.ts` to point to your Raspberry Pi:

```typescript
export const API = {
  BASE_URL: `http://<raspberry-pi-ip>/api`,  // or your domain
  // ... rest of config
};
```

Rebuild and redeploy the frontend.

## Step 10: Firewall Configuration

```bash
# Allow HTTP/HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable
```

## Step 11: SSL/HTTPS (Optional but Recommended)

Install Certbot for Let's Encrypt:

```bash
sudo apt install -y certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

## Monitoring and Maintenance

### View API logs:
```bash
sudo journalctl -u peerlearn-api -f
```

### Restart services:
```bash
sudo systemctl restart peerlearn-api
sudo systemctl restart nginx
```

### Check service status:
```bash
sudo systemctl status peerlearn-api
sudo systemctl status nginx
sudo systemctl status postgresql
```

## Troubleshooting

1. **API won't start**: Check logs with `journalctl -u peerlearn-api`
2. **Database connection issues**: Verify PostgreSQL is running and connection string is correct
3. **Frontend not loading**: Check Nginx error logs: `sudo tail -f /var/log/nginx/error.log`
4. **Port conflicts**: Ensure port 5106 is available for the API

## Performance Tips

1. **Use SSD instead of microSD** for better database performance
2. **Increase swap space** if using 4GB RAM model
3. **Use a cooling solution** to prevent thermal throttling
4. **Monitor resource usage**: `htop` or `top`

## Backup Strategy

1. **Database backup**:
   ```bash
   sudo -u postgres pg_dump peerlearndb > backup_$(date +%Y%m%d).sql
   ```

2. **Application files**: Regularly backup `/home/pi/peerlearn-api` and `/home/pi/peerlearn-frontend`

## Next Steps

- Set up automated backups
- Configure monitoring (e.g., Prometheus + Grafana)
- Set up domain name and DNS
- Configure email service for OTP functionality
- Set up log rotation

