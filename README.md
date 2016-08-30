[![Build status](https://ci.appveyor.com/api/projects/status/l8lcjqu5q0ld1je9?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe-4kmo4)
[![Documentation Status](https://readthedocs.org/projects/syringe/badge/?version=latest)](http://syringe.readthedocs.io/en/latest/?badge=latest)
[![Coverage Status](https://coveralls.io/repos/github/TotalJobsGroup/syringe/badge.svg?branch=master)](https://coveralls.io/github/TotalJobsGroup/syringe?branch=master)
[![MyGet](https://img.shields.io/myget/syringe/v/Syringe.svg?maxAge=2592000)]() 
[![Average time to resolve an issue](http://isitmaintained.com/badge/resolution/TotalJobsGroup/Syringe.svg)](http://isitmaintained.com/project/TotalJobsGroup/Syringe "Average time to resolve an issue") 
[![Percentage of issues still open](http://isitmaintained.com/badge/open/TotalJobsGroup/Syringe.svg)](http://isitmaintained.com/project/TotalJobsGroup/Syringe "Percentage of issues still open")

# Syringe
Syringe is a .NET automated HTTP testing tool for headless, Javascript-ignorant tests.

## Syringe's purpose in the universe

Syringe is a HTTP runner - if you can reach a server endpoint via HTTP, Syringe will be able to test it. It's purpose is:

1. To check canary pages (HTML, XML, JSON, TXT etc.)
2. To perform advanced smoke testing - get a HTML page (or other text-based resource) and assert that it contains some text.
3. Basic end-to-end HTTP tests. For example logging into a website and checking a page.

## Help
Our help files are hosted at [ReadTheDocs](http://syringe.readthedocs.io/en/latest/?badge=latest).

## Installation

### Pre-requisites

Make sure you have IIS enabled. 

##### Chocolatey

* Install chocolatey
* Install nuget command line : `choco install nuget.commandline`
* Powershell 4+: `choco install powershell4`

##### Mongodb: 

    # Work around for bug in the mongodb Chocolately package
    $env:systemdrive = "C:\ProgramData\chocolatey\lib\mongodata"
    choco install mongodb

##### Install Syringe via Chocolatey at myget 

*Note: this will configure Syringe on port 80. You should remove any site you have on Port 80, or pass in arguments to use a different port if you don't want to use 80.*

    choco source add -n "myget" -s "https://www.myget.org/F/syringe/api/v2"
    choco install syringe

Or if you want to configure Syringe to use custom settings (restoreConfigs will copy your configs over the package ones once the package is installed):

	choco install syringe -packageParameters "/websitePort:82 /websiteDomain:'www.example.com' /restoreConfigs:true"

##### Configure an OAuth2 provider

Syringe uses OAuth2 for its security. Currently it only supports Github, Google and Microsoft OAuth2 providers.

* [Register an Syringe OAuth2 app in Github](https://github.com/settings/developers). The callback url should be `http://localhost:1980`
* Edit the configuration.json file in the service directory to use the OAuth2 client id/secret.

##### Start the service

The Syringe REST API runs as Windows service, which can also be run as a command line app. This API is used to run all tests and is the data repository, it runs its own embedded HTTP server.

* Run `.\start-service.ps1` 
* Browse to http://localhost:1980 and login.

#### Building from source

Once you've cloned the repository, run `setup.ps`, this will:

* Build the solution, restoring the nuget packages  
* Create an IIS site
* Create C:\syringe folder with an example file.

Follow the "Configure OAuth" and "Start the service" steps above

#### LiteDB data dump example:

 	tools\LiteDB.Shell.exe --exec "open ../syringe.db" --exec "dump export.json" --exit
