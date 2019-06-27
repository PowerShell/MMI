#!/usr/bin/env sh

# Install OMI
wget https://github.com/microsoft/omi/releases/download/v1.6.0/omi-1.6.0-0.ssl_110.ulinux.x64.deb -O omi-1.6.0-0.ssl_110.ulinux.x64.deb
sudo apt-get install cron -y
sudo apt install ./omi-1.6.0-0.ssl_110.ulinux.x64.deb

# Build MMI
PATH=$PATH:~/.dotnet dotnet build -f netstandard2.0 src/Microsoft.Management.Infrastructure/ -c Linux -o bin
