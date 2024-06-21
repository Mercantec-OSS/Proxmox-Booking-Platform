#!/usr/bin/bash
PROJECT_PATH=$(pwd)

cd $PROJECT_PATH
git pull
cd $PROJECT_PATH/automatization/vmware-automation
git pull
cd $PROJECT_PATH/backend/vmware-backend
git pull
cd $PROJECT_PATH/frontend/vmware-frontend
git pull

cd $PROJECT_PATH
docker compose down
docker compose up -d