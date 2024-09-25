#!/usr/bin/bash

# Check if the backup file exists
BACKUP_FILE=$(find /restore/ -name 'backup_*' | head -n 1)
if [ "$BACKUP_FILE" != "" ]
then
    echo "-------------------------------------"
    echo "Restore backup from file $BACKUP_FILE"

    echo "Checking if the database $DATABASE exists..."
    mysql -u $USER -p"$PASSWORD" -P $PORT -e "DROP DATABASE IF EXISTS $DATABASE;"

    echo "Creating database $DATABASE..."
    mysql -u $USER -p"$PASSWORD" -P $PORT -e "CREATE DATABASE $DATABASE;"

    echo "Importing backup from $backup_file into database $DATABASE..."
    mysql -u $USER -p"$PASSWORD" -P $PORT $DATABASE < "$BACKUP_FILE"

    rm -f "$BACKUP_FILE"
    echo "-------------------------------------"
fi