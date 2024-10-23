#!/usr/bin/bash

if [ ! -f "/build/app" ]
then
    #build app
    cd /app
    dotnet publish --sc -o /build
fi

#run app
cd /build
/build/app
