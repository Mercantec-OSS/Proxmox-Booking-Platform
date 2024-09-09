#!/bin/sh
rm /root/.ssh/known_hosts

# check for 1 argument
if [ $# -ne 1 ]
then
    echo "Incorrect number of arguments."
    echo "Usage:"
    echo "./create_backup.sh '<username>__<password>__<esxi_host>'"
    echo "Example:"
    echo "./create_backup.sh 'root__password__10.1.60.120'"
    exit 1
fi

# Input string
username=$(echo "$1" | awk -F'__' '{print $1}')
password=$(echo "$1" | awk -F'__' '{print $2}')
address=$(echo "$1" | awk -F'__' '{print $3}')

# Pass the local variable to the remote script
sshpass -p "$password" ssh -o StrictHostKeyChecking=accept-new "$username"@"$address" "rm -r /etc/vmware/license.cfg"
sshpass -p "$password" ssh -o StrictHostKeyChecking=accept-new "$username"@"$address" "cp /etc/vmware/.#license.cfg /etc/vmware/license.cfg"

