#!/usr/bin/env bash
set -e
cd "$(dirname "$0")"

echo "This script must be run while Unity is CLOSED."
echo "Removing generated Unity caches only..."

rm -rf Library
rm -rf Temp
rm -rf obj

# Do not delete Assets, Packages or ProjectSettings.
rm -f Packages/packages-lock.json

echo "Done."
echo "Reopen this project in Unity 6000.5.9f1."
echo "Package Manager should resolve com.unity.inputsystem 1.20.0."
