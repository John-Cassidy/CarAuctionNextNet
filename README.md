# CarAuctionNextNet

How to build a microservices based app using .Net, NextJS, IdentityServer, RabbitMQ running on Docker and Kubernetes

## dotnet tools

```powershell
dotnet tool list -g
dotnet tool install --global [TOOL_NAME] --version x.x.x
dotnet tool update --global [TOOL_NAME] #updates to latest version
```

## AuctionService - Postgres DB

### dotnet-ef - Use for Migrations

Install globally

```powershell
dotnet tool install --global dotnet-ef --version 8.0.6
```

#### dotnet ef migrations (option 1)

When setting up development for first time with PostgreSQL container, run manual migration from command line in Project folder.

```powershell
dotnet ef migrations add InitialCreate -o Data/Migrations
```

Where InitialCreate is the name that we will give our migration, you can change this name with details of what your migration refers to, such as changing a field in a table, adding or removing fields, by convention we try to detail the update that the migration will do.

-p (project) we pass the name of the solution that contains our DbContext in the case of Infrastructure

-s (solution) we pass our main project in the case of the API

If everything went well after running the command you will get a message like this: 'Done. To undo this action, use ‘ef migrations remove’'

##### Migrations remove command

The migrations remove command is used to remove the created migration if it is not as you wanted.

```powershell
dotnet ef migrations remove -o Data/Migrations
```

##### Migrations database drop command

Drop / Delete database associated with the DbContext in the AuctionService project. This is typically done when you want to completely reset the database, for example during development when you want to recreate the database from scratch.

```powershell
dotnet ef database drop
```

##### Apply pending migrations to the database

To apply any pending migrations to the database, effectively updating the database schema. If no migrations are pending, this command has no effect.

```powershell
dotnet ef database update
```

## Dockerfile

Build AuctionService image: open prompt in solution folder and run the following command:

```powershell
# docker build --force-rm -t auctionservice:dev --target base --build-arg "BUILD_CONFIGURATION=Debug" -f "./backend/src/Auction/AuctionService/Dockerfile" .
docker build --force-rm -t auctionservice:latest -f "./backend/src/Auction/AuctionService/Dockerfile" .

docker run -d -p 8080:80 --name auctionservice auctionservice
```

```powershell
# NOTE: REBUILD IMAGES TO INCLUDE CODE CHANGES AND START
docker-compose -f docker-compose.yml up --build

# OR REBUILD IMAGES USING NOCACHE OPTION:
docker-compose -f docker-compose.yml build --no-cache
docker-compose -f docker-compose.yml up

# NOTE: START CONTAINERS FROM EXISTING IMAGES WITHOUT REBUILDING
docker-compose -f docker-compose.yml up -d
# NOTE: STOP RUNNING CONTAINERS AND REMOVE CONTAINERS
docker-compose -f docker-compose.yml down
```

## SearchService - MongoDB

Nuget packages:

- MongoDB.Entities: [MongoDB.Entities Project URL](https://mongodb-entities.com/)
- Polly:

  - [https://github.com/App-vNext/Polly](https://github.com/App-vNext/Polly)
  - [https://www.thepollyproject.org/](https://www.thepollyproject.org/)

- Microsoft.Extensions.Http.Resilience (Replaces Microsoft.Extensions.Http.Polly): [Docs on Replacing Polly with this Package](https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/)

The new HTTP resilience packages are build upon the foundations of Polly, presenting the .NET community with dedicated and refined HTTP-based resilience APIs.

Resilience packages:

- Microsoft.Extensions.Resilience: This package provides a minimal set of APIs. Its primary purpose is to enrich Polly’s metrics using the AddResilienceEnricher extension for IServiceCollection. For more details, refer to the Resilience docs.
- Microsoft.Extensions.Http.Resilience: This package offers HTTP-specific APIs that integrate with Polly v8 and IHttpClientFactory. It is the successor to the Microsoft.Extensions.Http.Polly package and is recommended for all new projects. See the section below for more information.
- Microsoft.Extensions.Http.Polly: This package integrates older versions of Polly with HttpClient and IHttpClientFactory.

## RabbitMQ

### Overview

Message Broker used for asynchronous communication between microservices.

Image: rabbitmq:3-management-alpine

- Message Broker
- Management Utility

Type of Exchange: Fanout

Nuget packages:

- MassTransit.RabbitMQ: [https://masstransit.io/](https://masstransit.io/)

### RabbitMQ Management

[Local Development URL:](http://localhost:15672/)
username: guest
password: guest

### MassTransit

Used by microservices to communicate with each other via RabbitMQ.

- Message Routing: Type-based publish/subscribe and automatic broker topology configuration
- Exception Handling: When an exception is thrown, messages can be retried, redelivered, or moved to an error queue
- Test Harness: Fast, in-memory unit tests with consumed, published, and sent message observers
- Dependency Injection: Service collection configuration and scope service provider management
