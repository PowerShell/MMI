#!/usr/bin/env sh
export PATH=$PATH:$(pwd)/bin
echo $PATH
cp bin/libmi.so test/Microsoft.Management.Infrastructure.Tests/bin/
src/omi/Unix/output/bin/omiserver -i -d --livetime 120 --httpport 0 --httpsport 0
PATH=$PATH:~/.dotnet dotnet test test/Microsoft.Management.Infrastructure.Tests/ -c Linux -o bin
