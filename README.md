# Marketplace

This is the lab repository for the Practical Event-Sourcing with C# hands-on session. The workshop is taught by [Alexey](https://twitter.com/Zimareff) and [Sergio](https://twitter.com/ragingkore). Feel free to ping us if you have questions after the session or feedback to improve it.

Due to the time limitation, there are many aspects of event-sourcing, which we will not be able to cover. Some reference materials will be included in this file later on, so start the repo to be informed.

## Prerequisites

- .NET Core 2
- C# IDE or editor of your choice (for example Visual Studio 2017, Rider or VS Code)
- Docker Compose

The solution is using C# 7.1 features so if you are using Visual Studio - ensure you have VS 2017. Rider supports the latest C# by default or will ask you if you want to enable it.

Note that Docker Compose might _not_ included to your version of Docker, so you need to [download and install](https://docs.docker.com/compose/install/) it. [Docker](https://docs.docker.com/install/) is a pre-requisite for Docker Compose.

> In case you are unable to use Docker Compose, please download the following products:
>- [EventStore](https://eventstore.org/downloads/)
>- [RavenDB 4.0 RC](https://ravendb.net/downloads)

*Important:* carefully read this section and ensure that everything works on your machine _before_ the workshop. When using Docker Compose, downloading images requires significant bandwidth and we will not be able to do it during the workshop due to the venue WiFi capacity.

### Using Docker Compose

You can run the required infrastructure components by issuing a simple command:

```
$ docker-compose up
```

form your terminal command line, whilst being inside the repository rot directory.

It might be a good idea to run the services in detached mode, so you don't accidentally stop them. To do this, execute:

```
$ docker-compose up -d
```

To stop all services, from the repository root execute this command:

```
$ docker-compose stop
```

Hence that this command does not remove containers so you can run them again using `docker-compose up` or `docker-compose start` and your data will be retained from the previous section.

If you want to clean up all data, use

```
$ docker-compose down
```

This command stops all services and removes containers. The images will still be present locally so when you do `docker-compose up` - containers will be created almost instantly and everything will start with clean volumes.

### Installing components manually

If you are unable or do not with to use Docker Compose, you can download EventStore and RavenDb and the install these products locally. EventStore has no "installation" as such on Windows, you just need to extract the archive and start the executable.

EventStore installation instructions for Ubuntu is located [here](https://eventstore.org/docs/server/installing-from-debian-repositories/)
and instructions for running a Windows instance can be found [here](https://eventstore.org/docs/server/).

RavenDb 4.0RC can be downloaded for all platform using the link above.

Please reconsider using Docker Compose, since it is the easiest way to get started without installing anything.

You can use an existing installation of the EventStore and build your read models in a database of your choice but we provide examples only for EventStore and RavenDb. Of course, you should not feel restricted by these limitations and outside the workshop please feel free to experiment with different tools.

## Structure

The workshop source files are located in the `src` folder and there are several stages:

- `01-` is to get things started
- `02-` shows how events are being persisted
- `03-` explains how to build your first read model
- `04-` demonstrates how event-sourced aggregates can be tested
- `05-` is where we build another read model

Each section contains two folders - `before` and `after`. The first one contains the starting point for the section and the second one shows the completed code for the section. Of course, even the `after` version has a lot of potential for being improved.

## What to do

Start with trying to get up the Docker Compose images. Do this before the workshop, you will not have enough time to install everything during the workshop or just before it because the venue WiFi will probably not allow you to complete the process within reasonable time.

Start Docker Compose as described above while at home and check that the images are downloaded and everything starts properly. Check if EventStore and RavenDb respond via http by visiting the administration consoles:

- [EventStore](http://localhost:2113), user name is "admin" and password is "changeit"
- [RavenDb](http://localhost:8080)
