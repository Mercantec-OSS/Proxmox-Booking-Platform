#!/bin/bash

if [ "$#" -ne 2 ]; then
    echo "Usage: $0 <database_name> <backup_file>"
    exit 1
fi

db_name=$1
backup_file=$2

if [ ! -f "$backup_file" ]; then
  echo "File $backup_file not found!"
  exit 1
fi

echo "Checking if the database $db_name exists..."
mysql -u root -p -e "DROP DATABASE IF EXISTS $db_name;"

echo "Creating database $db_name..."
mysql -u root -p -e "CREATE DATABASE $db_name;"

echo "Importing backup from $backup_file into database $db_name..."
mysql -u root -p $db_name < "$backup_file"

echo "Process completed."