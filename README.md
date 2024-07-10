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

### Outbox Pattern

[Overview](https://masstransit.io/documentation/patterns/transactional-outbox)
[Configuration](https://masstransit.io/documentation/configuration/middleware/outbox#configuration)

Add Nuget Package to AuctionService: MassTransit.EntityFrameworkCore

Use this package to configure and store the message being sent to message broker in PostgreSQL db. Once the message is successfully sent to message broker, remove message from PostgreSQL db.

```csharp
// AuctionService => Program.cs
// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    // add entityframework outbox pattern
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

// AuctionService => AuctionDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add MassTransit - Outbox Pattern tables to PostgreSQL DB
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

// AuctionService => AuctionsController => CreateAuction API
// Using the Outbox pattern, MessageBroker messages are saved in the Outbox table
// and are only sent to the MessageBroker after the DB transaction is committed
// NOTE: if the DB transaction fails, the message will not be sent
await _context.Auctions.AddAsync(auction);
var newAuction = _mapper.Map<AuctionDto>(auction);
await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
var result = await _context.SaveChangesAsync() > 0;

// SearchService => Consumers
// Consume the MessageBroker messages of type: AuctionCreated
public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

        var item = _mapper.Map<Item>(context.Message);

        await item.SaveAsync();
    }
}
```

```powershell
# Add migration to AuctionService
dotnet ef migrations add Outbox -o Data/Migrations

# To apply any pending migrations to the database, effectively updating the database schema.
# If no migrations are pending, this command has no effect.
dotnet ef database update
```

## IdentityServer using Duende.IdentityServer

Duende IdentityServer v7 for .NET 8

Duende Software company offers IdentityServer dotnet project templates as a flexible and standards-compliant OpenID Connect and OAuth 2.x framework for ASP.NET Core.

[Community Edition: A free license that is feature equivalent to our Enterprise Edition](https://duendesoftware.com/products/communityedition)

[IdentityServer v7 Documentation](https://docs.duendesoftware.com/identityserver/v7/)

[GitHub Repo](https://github.com/DuendeSoftware/IdentityServer)

[Templates](https://github.com/DuendeSoftware/IdentityServer.Templates)

Overview:

- Create IdentityServer project
- Customize IdentityServer for our needs
- Adding authentication to aour API endpoints
- Implements OpenID Connect (OIDC) and OAuth2 as Single Sign (SSO) On Solution using OAuth2 Flow

Infrastructure:

- Duende IdentityServer with ASP.NET Core Identity
- PostgresSql DB Server

Nuget Packages: (added to the default template)

- Npgsql.EntityFrameworkCore.PostgreSQL
- Polly

### Install/Update Project Templates

```powershell
> dotnet new install Duende.IdentityServer.Templates

The following template packages will be installed:
   Duende.IdentityServer.Templates

Duende.IdentityServer.Templates (version 6.3.1) is already installed, it will be replaced with latest version.
Duende.IdentityServer.Templates::6.3.1 was successfully uninstalled.
Success: Duende.IdentityServer.Templates::7.0.4 installed the following templates:
Template Name                                               Short Name     Language  Tags
----------------------------------------------------------  -------------  --------  ------------------
Duende BFF Host using a Remote API                          bff-remoteapi  [C#]      Web/IdentityServer
Duende BFF using a Local API                                bff-localapi   [C#]      Web/IdentityServer
Duende IdentityServer Empty                                 isempty        [C#]      Web/IdentityServer
Duende IdentityServer Quickstart UI (UI assets only)        isui           [C#]      Web/IdentityServer
Duende IdentityServer with ASP.NET Core Identity            isaspid        [C#]      Web/IdentityServer
Duende IdentityServer with Entity Framework Stores          isef           [C#]      Web/IdentityServer
Duende IdentityServer with In-Memory Stores and Test Users  isinmem        [C#]      Web/IdentityServer
```

### Project Creation

run following command from `Infrastructure/` folder:

```powershell
dotnet new isaspid -o IdentityService --dry-run

The template "Duende IdentityServer with ASP.NET Core Identity" was created successfully.

Processing post-creation actions...
Template is configured to run the following action:
Actual command: dotnet run /seed
Do you want to run this action [Y(yes)|N(no)]?
N

# add IdentityService project to sln
```

### PostgreSQL

Use the PostreSQL running in container used by AuctionService.

Run commands below from IdentityService folder

```powershell
# Create Migrations
dotnet ef migrations add InitialCreate -o Data/Migrations
```

### Register New Users

Create razor page to Register new users!

### Issue Tokens based on User Passwords

Add client credentials to allow clients to request a token. This is not a best practice but will allow Postman to generate a token for development testing purposes.

[Documentation](https://docs.duendesoftware.com/identityserver/v7/tokens/password_grant/)

### Add Custom Profile Service to IdentityService

## API Gateway Service

Yarp: Yet Another Reverse Proxy

[Yarp Documentation](https://microsoft.github.io/reverse-proxy/)

[Yarp Getting Started](https://microsoft.github.io/reverse-proxy/articles/getting-started.html)

Used for:

- Reverse Proxy
- Security
- URL Rewriting

### Configuration Files

[Configuration Files Documentation](https://microsoft.github.io/reverse-proxy/articles/config-files.html)

The reverse proxy can load configuration for routes and clusters from files using the IConfiguration abstraction from Microsoft.Extensions. The project will use appsettings.JSON. The configuration will also be updated without restarting the proxy if the source file changes.
