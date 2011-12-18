#!/bin/bash
# Recursively generates sprite sheets for subdirectories.
if (($# != 1)); then
    echo "Usage: $0 directory_path"
    exit 1
fi;

find "$1" -mindepth 1 -type d -print0 | xargs -0 -I "{}" ./gensheet.sh {}

