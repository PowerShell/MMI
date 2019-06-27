#!/usr/bin/env sh
mkdir -p test/Microsoft.Management.Infrastructure.Tests/bin/
cp /opt/omi/lib/libmi.so test/Microsoft.Management.Infrastructure.Tests/bin/
/opt/omi/bin/omiserver -i -d --livetime 120 --httpport 0 --httpsport 0
PATH=$PATH:~/.dotnet dotnet test test/Microsoft.Management.Infrastructure.Tests/ -c Linux -o bin --logger "trx;LogFileName=TestResults.trx"
