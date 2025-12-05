# Raspberry Pi 4 Deployment Checklist

## What You Need to Do

### 1. Hardware Setup
- [ ] Raspberry Pi 4 (4GB+ RAM recommended)
- [ ] MicroSD card (32GB+)
- [ ] Power supply
- [ ] Network connection (Ethernet cable recommended)

### 2. Software Installation on Pi
- [ ] Flash Raspberry Pi OS (64-bit) to SD card
- [ ] Enable SSH
- [ ] Update system: `sudo apt update && sudo apt upgrade`
- [ ] Install .NET 9.0 Runtime (ARM64)
- [ ] Install PostgreSQL
- [ ] Install Node.js and pnpm (for building frontend)
- [ ] Install Nginx

### 3. Database Setup

**Option A: Remote SQL Server (Recommended if you have existing database)**
- [ ] Update connection string to point to remote SQL Server
- [ ] Ensure SQL Server allows remote connections
- [ ] Open SQL Server port (1433) in firewall
- [ ] Test connection from Raspberry Pi
- [ ] Run EF Core migrations (if needed)

**Option B: PostgreSQL on Pi (Only if hosting database on Pi)**
- [ ] Install PostgreSQL on Raspberry Pi
- [ ] Switch from SQL Server to PostgreSQL
- [ ] Update `Infrastructure.csproj` - replace SqlServer package with Npgsql
- [ ] Update `Program.cs` - change `UseSqlServer` to `UseNpgsql`
- [ ] Update connection string format
- [ ] Create PostgreSQL database and user
- [ ] Run EF Core migrations

### 4. Backend Deployment
- [ ] Build backend for ARM64: `dotnet publish -r linux-arm64`
- [ ] Transfer files to Raspberry Pi
- [ ] Create `.env` file with PostgreSQL connection string
- [ ] Create systemd service file
- [ ] Enable and start the service
- [ ] Test API is running

### 5. Frontend Deployment
- [ ] Update `config.ts` with Raspberry Pi IP/domain
- [ ] Build frontend: `pnpm build`
- [ ] Transfer `dist` folder to Raspberry Pi
- [ ] Configure Nginx to serve frontend

### 6. Nginx Configuration
- [ ] Create Nginx config file
- [ ] Configure reverse proxy for `/api` endpoint
- [ ] Configure WebSocket proxy for `/hubs` (SignalR)
- [ ] Configure static file serving for frontend
- [ ] Test and restart Nginx

### 7. Security & Production
- [ ] Set up firewall rules
- [ ] Configure SSL/HTTPS (Let's Encrypt)
- [ ] Update CORS settings for production domain
- [ ] Set secure JWT secret key
- [ ] Disable Swagger in production

### 8. Monitoring & Maintenance
- [ ] Set up log rotation
- [ ] Create backup script for database
- [ ] Test service auto-restart on reboot
- [ ] Monitor resource usage

## Quick Command Reference

```bash
# Check API status
sudo systemctl status peerlearn-api

# View API logs
sudo journalctl -u peerlearn-api -f

# Restart API
sudo systemctl restart peerlearn-api

# Check Nginx status
sudo systemctl status nginx

# Test Nginx config
sudo nginx -t

# Restart Nginx
sudo systemctl restart nginx

# Database backup
sudo -u postgres pg_dump peerlearndb > backup.sql
```

## Important Notes

1. **Database Options**: 
   - **Remote SQL Server**: If you have SQL Server on another server, you can keep using it! Just update the connection string. ✅ **Easiest option**
   - **PostgreSQL on Pi**: Only needed if you want to host the database on the Raspberry Pi itself
2. **Build for ARM64**: Always build with `-r linux-arm64` flag
3. **Connection String**: Update to PostgreSQL format
4. **Ports**: Default API port is 5106, ensure it's not blocked
5. **Performance**: Consider using SSD instead of microSD for database

## Estimated Time

- Initial setup: 2-3 hours
- Migration and testing: 1-2 hours
- Deployment: 1 hour
- **Total: 4-6 hours** (first time)

