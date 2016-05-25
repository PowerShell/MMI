#!/usr/bin/env sh
dotnet restore -v Warning
dotnet build -f netstandard1.5 src/Microsoft.Management.Infrastructure/
