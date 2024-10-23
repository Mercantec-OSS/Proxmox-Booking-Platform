#!/bin/bash

if [ "$#" -ne 3 ]; then
    echo "Usage: $0 <database_name> <backup_file> <db_port>"
    exit 1
fi

db_name=$1
backup_file=$2
port=$3

read -sp "Enter MySQL root password: " db_password
echo

echo "Checking if the database $db_name exists..."
mysql -u root -p"$db_password" -P $port -e "DROP DATABASE IF EXISTS $db_name;"

echo "Creating database $db_name..."
mysql -u root -p"$db_password" -P $port -e "CREATE DATABASE $db_name;"

echo "Importing backup from $backup_file into database $db_name..."
mysql -u root -p"$db_password" -P $port $db_name < "$backup_file"

echo "Process completed."