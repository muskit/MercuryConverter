#!/usr/bin/env sh

cd $(dirname $0)

app_version=MercuryConverter-v$(cat src/Assets/version)

# create builds
dotnet publish src --runtime linux-x64 -c Release -p:PublishSingleFile=True --self-contained false -o "./builds/$app_version-linux-x64"
dotnet publish src --runtime win-x64 -c Release -p:PublishSingleFile=True --self-contained false -o "./builds/$app_version-win-x64"

# package builds
cd builds
zip -r "$app_version-linux-x64.zip" "$app_version-linux-x64"
zip -r "$app_version-win-x64.zip" "$app_version-win-x64"
