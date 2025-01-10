#!/usr/bin/bash
user=$(sudo whoami)
if [ $user != "root" ]; then
    echo "You need for root permissions"
    exit 1
fi

git pull

sudo rm -rf ./backend/build/*
sudo rm -rf ./frontend/build/*

docker compose restart