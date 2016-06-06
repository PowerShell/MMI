#!/usr/bin/env sh

# Build OMI
(
    cd src/omi/Unix
    ./configure --dev
    make -j
)

# Copy libmi
mkdir -p bin
cp src/omi/Unix/output/lib/libmi.so bin

# Build MMI
dotnet restore -v Warning
dotnet build -f netstandard1.5 src/Microsoft.Management.Infrastructure/ -c Linux -o bin
