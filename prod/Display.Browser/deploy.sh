#!/bin/bash

CONFIGURATION="Release"
REMOTE_USER="cdl"
REMOTE_HOST="opicdl"

set -ex

dotnet publish -c $CONFIGURATION

rsync -avz --rsync-path="sudo rsync" \
  "./bin/$CONFIGURATION/net8.0-browser/publish/wwwroot/" \
  "$REMOTE_USER@$REMOTE_HOST:/var/www/html/"
