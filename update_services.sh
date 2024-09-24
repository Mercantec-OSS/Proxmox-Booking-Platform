#!/usr/bin/bash
git pull

sudo rm -rf ./automatization/build/*
sudo rm -rf ./backend/build/*
sudo rm -rf ./frontend/build/*

docker compose restart