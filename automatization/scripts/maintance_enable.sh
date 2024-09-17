#!/bin/sh
rm /root/.ssh/known_hosts

# check for 1 argument
if [ $# -ne 1 ]
then
    echo "Incorrect number of arguments."
    echo "Usage:"
    echo "./maintance_enable.sh '<username>__<password>__<esxi_host>'"
    exit 1
fi

# Input string
username=$(echo "$1" | awk -F'__' '{print $1}')
password=$(echo "$1" | awk -F'__' '{print $2}')
address=$(echo "$1" | awk -F'__' '{print $3}')

# Pass the local variable to the remote script
sshpass -p "$password" ssh -o StrictHostKeyChecking=accept-new "$username"@"$address" "vim-cmd hostsvc/maintenance_mode_enter"
