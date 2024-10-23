#!/bin/bash
CURRENT_DATE=$(date +%Y-%m-%d_%H-%M-%S)
FILE_NAME="backup_$CURRENT_DATE.sql"

mariadb-dump -h $HOST -P $PORT -u $USER -p$PASSWORD $DATABASE > /backup/$FILE_NAME