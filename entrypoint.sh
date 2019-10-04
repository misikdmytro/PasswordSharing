#!/bin/bash

set -e
run_cmd="dotnet run --urls http://*:8080 --no-launch-profile"

cd PasswordSharing.Web

exec $run_cmd
