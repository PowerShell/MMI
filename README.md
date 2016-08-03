Microsoft.Management.Infrastructure
===================================

[![Build Status](https://travis-ci.com/PowerShell/MMI.svg?token=31YifM4jfyVpBmEGitCm&branch=johnkord-mmi_changes)](https://travis-ci.com/PowerShell/MMI)

This repo contains the `Microsoft.Management.Infrastructure` source code (and
soon tests). This is a dependency of PowerShell, but is kept separate.

Environment
===========

Toolchain Setup
---------------

MMI requires the following packages:

```sh
sudo apt-get install libpam0g-dev libssl-dev libcurl4-openssl-dev
```

Also install [PowerShell][] from the latest release per their instructions.

[powershell]: https://github.com/PowerShell/PowerShell

Git Setup
---------

MMI has a submodule, so clone recursively.

```sh
git clone --recursive git@github.com:PowerShell/MMI.git
```

Build
-----
```sh
./build.sh
```

Test
----
```sh
./test.sh
```
