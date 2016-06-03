#!/usr/bin/env sh
src/omi/Unix/output/bin/omiserver -i -d --livetime 120 --httpport 0 --httpsport 0
dotnet test test/Microsoft.Management.Infrastructure.Tests/ -c Linux -o bin
