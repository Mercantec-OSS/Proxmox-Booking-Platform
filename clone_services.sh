#!/usr/bin/bash
PROJECT_PATH=$(pwd)

cd $PROJECT_PATH
git clone https://gitlab.pcvdata.dk/team/vmware-docker-compose.git
cd $PROJECT_PATH/automatization
git clone https://gitlab.pcvdata.dk/team/vmware-automation.git
cd $PROJECT_PATH/backend
git clone https://gitlab.pcvdata.dk/team/vmware-backend.git
cd $PROJECT_PATH/frontend
git clone https://gitlab.pcvdata.dk/team/vmware-frontend.git