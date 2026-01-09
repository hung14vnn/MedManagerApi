#!/bin/bash

# Database backup script
# Runs daily at 2 AM

BACKUP_DIR="/backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="${BACKUP_DIR}/medmanager_backup_${TIMESTAMP}.sql"
KEEP_DAYS=${BACKUP_KEEP_DAYS:-7}

# Create backup
echo "Starting backup at $(date)"
PGPASSWORD=$POSTGRES_PASSWORD pg_dump -h db -U $POSTGRES_USER $POSTGRES_DB > $BACKUP_FILE

# Compress backup
gzip $BACKUP_FILE

# Delete old backups
find $BACKUP_DIR -name "medmanager_backup_*.sql.gz" -mtime +$KEEP_DAYS -delete

echo "Backup completed: ${BACKUP_FILE}.gz"
echo "Old backups (older than $KEEP_DAYS days) deleted"

# Setup cron job
echo "0 2 * * * /backup.sh >> /var/log/backup.log 2>&1" | crontab -
