#!/usr/bin/bash
$user=$(whoami)
if [ $user != "root" ]; then
    echo "You must run this script as root"
    exit 1
fi

git pull

sudo rm -rf ./automatization/build/*
sudo rm -rf ./backend/build/*
sudo rm -rf ./frontend/build/*

docker compose restart