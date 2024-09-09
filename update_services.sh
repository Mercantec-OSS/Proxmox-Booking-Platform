#!/usr/bin/bash
PROJECT_PATH=$(pwd)

git pull
docker compose down
docker compose up -d --build