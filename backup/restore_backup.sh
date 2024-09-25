#!/usr/bin/bash

# Check if the backup file exists
BACKUP_FILE=$(find /restore/ -name 'backup_*' | head -n 1)
if [ "$BACKUP_FILE" != "" ]
then
    echo "-------------------------------------"
    echo "Restore backup from file $BACKUP_FILE"

    echo "Checking if the database $DATABASE exists..."
    mysql -h $HOST -P $PORT -u $USER -p"$PASSWORD" -e "DROP DATABASE IF EXISTS $DATABASE;"

    echo "Creating database $DATABASE..."
    mysql -h $HOST -P $PORT -u $USER -p"$PASSWORD" -e "CREATE DATABASE $DATABASE;"

    echo "Importing backup from $backup_file into database $DATABASE..."
    mysql -h $HOST -P $PORT -u $USER -p"$PASSWORD" $DATABASE < "$BACKUP_FILE"

    rm -f "$BACKUP_FILE"
    echo "-------------------------------------"
fi