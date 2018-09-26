#!/bin/bash

# Ensure build is created
mkdir -p build

# Remove exisiting build folder if it exists
rm -r -f ./build/$1

# publish into build folder specified from command line arg $1
dotnet publish ./Steamline.co.Api/Steamline.co.Api.csproj --framework netcoreapp2.1 --runtime ubuntu.16.04-x64 -o ../build/$1

# build docs 
#./build_docs.sh $1

# Remove local and development configs if they exist
rm -f ./build/$1/appsettings.local.json ./build/$1/appsettings.Development.json