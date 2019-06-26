#!/usr/bin/env sh

# Build OMI
#(
#    cd src/omi/Unix
#    ./configure --dev
#    make -j
#)

# Copy libmi
#mkdir -p bin
#cp src/omi/Unix/output/lib/libmi.so bin

# Install OMI
wget https://github.com/microsoft/omi/releases/download/v1.6.0/omi-1.6.0-0.ssl_100.ulinux.x64.deb -O omi-1.6.0-0.ssl_100.ulinux.x64.deb
sudo apt-get install cron -y
sudo apt install ./omi-1.6.0-0.ssl_100.ulinux.x64.deb

# Build MMI
PATH=$PATH:~/.dotnet dotnet build -f netstandard2.0 src/Microsoft.Management.Infrastructure/ -c Linux -o bin
