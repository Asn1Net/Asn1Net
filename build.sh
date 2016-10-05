#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi

dotnet restore ./src/Asn1Net
dotnet build ./src/Asn1Net --configuration Release

dotnet restore ./test/Asn1Net.Test
dotnet build ./test/Asn1Net.Test --configuration Release

# Ideally we would use the 'dotnet test' command to test netcoreapp and net451 so restrict for now 
# but this currently doesn't work due to https://github.com/dotnet/cli/issues/3073 so restrict to netcoreapp

dotnet test ./test/Asn1Net.Test

revision=${TRAVIS_JOB_ID:=1}  
revision=$(printf "%04d" $revision) 

dotnet pack ./src/Asn1Net --configuration Release --output ./artifacts --version-suffix=$revision  