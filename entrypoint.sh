#!/bin/bash

set -e
url=$1
run_cmd="dotnet run --urls ${url} --no-launch-profile"

cd PasswordSharing.Web

exec $run_cmd
