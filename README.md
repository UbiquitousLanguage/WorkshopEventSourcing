# Marketplace

Write something here.

## Building the Solution

### Prerequisites

- [.NET Core 2.0](https://www.microsoft.com/net/download/windows)
- [Mono](http://www.mono-project.com/download/) `(MacOS/Linux only)`

> Consider running Docker containers:
>- [EventStore](https://eventstore.org/docs/introduction/4.0.0/)
>- [RavenDB 4.0 RC](https://ravendb.net/downloads)

#### Windows
ExecutionPolicy for Powershell set to RemoteSigned or Unrestricted.

`Set-ExecutionPolicy RemoteSigned -Scope Process`

#### Linux / MacOS
Adjust the permissions for the bootstrapper script.

`chmod +x build.sh`

### Build Script

#### From a terminal prompt

Command     | Description
:-----------| :----------
`build.ps1` | Windows build script
`build.sh`  | Linux / MacOS

#### Build targets

Target            | Description
:-----------------| :----------
`Clean`           | Clears all build outputs in 'build/artifacts' and all solution bin/obj folders.
`Restore`         | Restores the dependencies and tools of the project.
`Build`           | Builds the project and all of its dependencies.
`Test`            | Run all tests.