#!/bin/bash

# Ensure deploy dir exists
mkdir -p deploy

TARGET=$1.tar.gz

# Remove target if it exists
rm -r -f ./deploy/$TARGET

# Zip up dir
tar -zcvf ./deploy/$TARGET -C ./build $1
