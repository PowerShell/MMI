#!/usr/bin/env sh
dotnet restore -v Warning
dotnet build -f netstandard1.5
