#!/bin/bash
CURRENT_DATE=$(date +%Y-%m-%d_%H-%M-%S)
FILE_NAME="backup_$CURRENT_DATE.sql"

IFS=';' read -r -a params <<< "$DB_CONNECTION_STRING"

declare -A conn_params

for param in "${params[@]}"; do
  key=$(echo $param | cut -d'=' -f1)
  value=$(echo $param | cut -d'=' -f2)
  conn_params[$key]=$value
done

HOST=${conn_params[server]}
DATABASE=${conn_params[database]}
PORT=${conn_params[port]}
USER=${conn_params[user]}
PASSWORD=${conn_params[password]}

mariadb-dump -h $HOST -P $PORT -u $USER -p$PASSWORD $DATABASE > /backup/$FILE_NAME