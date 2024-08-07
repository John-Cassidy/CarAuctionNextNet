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

# CLEAN UP Docker Cache
docker system prune -a
docker volume prune
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

## Testing

```powershell
# Create Unit test project
dotnet new xunit -o backend/tests/AuctionService.UnitTests --dry-run

# Create Integration test project
dotnet new xunit -o backend/tests/AuctionService.IntegrationTests --dry-run
```

```csharp
public class AuctionEntityTests
{
    [Fact]
    public void Method_Scenario_ExpectedResult()
    {
        // arrange

        // act

        // assert
    }
}
```

Integration Testing Nuget Packages:

- Microsoft.AspNetCore.Mvc.Testing

  - Support for writing functional tests for MVC applications.
  - [Documentation](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0)

- Testcontainers.PostgreSql

  - Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions.
  - [Testcontainers Documentation](https://dotnet.testcontainers.org/)

- WebMotions.Fake.Authentication.JwtBearer

  - This code allows to fake a Jwt Bearer and build integration tests for an ASP.Net Core application. Using this, we can fake any authentication we need, without the need to really authenticate a user.
  - [Webmotion.Fake.Authentication.JwtBearer Documentation](https://github.com/webmotions/fake-authentication-jwtbearer)

## NextJS

## BidService

Create a .Net Web API project that will:

Controller endpoints:

- Creates a new bid for an auction using the auctionId and the amount of
  the bid. Returns Bid
- Returns a list of bids for an Auction

Events emitted:

- BidService.BidPlaced - When a bid has been placed in the BidService
- BidService.AuctionFinished - When an auction has reached the AuctionEnd date

Events consumed:

- AuctionService.AuctionCreated - When an auction has been created in the
  AuctionService

```powershell
dotnet new webapi -o backend/src/BiddingService --dry-run

File actions would have been taken:
  Create: backend\src\BiddingService\BiddingService.csproj
  Create: backend\src\BiddingService\BiddingService.http
  Create: backend\src\BiddingService\Program.cs
  Create: backend\src\BiddingService\Properties\launchSettings.json
  Create: backend\src\BiddingService\appsettings.Development.json
  Create: backend\src\BiddingService\appsettings.json

dotnet sln add backend/src/BiddingService
```

## Grpc Service / Client

Used by BidService to check AuctionService for any missing Auctions when a Bid comes in.

[Blog on How to Implement gRPC Client and Server in .NET 8](https://medium.com/@gabrieletronchin/how-to-implement-grpc-client-and-server-in-net-8-2b722b50c3b0)

[Blog Github Repository](https://github.com/GabrieleTronchin/GRPC.Sample)

[Microsoft Tutorial: Create a gRPC client and server in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-8.0&tabs=visual-studio-code)

![Grpc Server - Client](./.assets/images/001_grpc_server_client.png)

[VS Code Extension: vscode-proto3](https://github.com/zxh0/vscode-proto3)

### Nuget Packages

gRPC Server:

- Grpc.AspNetCore v2.64.0 - An ASP.NET Core framework for hosting gRPC services. gRPC on ASP.NET Core integrates with standard ASP.NET Core features like logging, dependency injection (DI), authentication and authorization

gRPC Client:

- Grpc.Net.Client v2.64.0 - A gRPC client for .NET Core that builds upon the familiar HttpClient. The client uses new HTTP/2 functionality in .NET Core
- Google.Protobuf v3.27.2, which contains protobuf message APIs for C#.
- Grpc.Tools v2.65.0, which contain C# tooling support for protobuf files. The tooling package isn't required at runtime, so the dependency is marked with PrivateAssets="All"

## SignalR Service

C:\DEV\github.com\CarAuctionNextNet\backend\src\Notification

```powershell
dotnet new web -o backend/src/Notification/NotificationService --dry-run

File actions would have been taken:
  Create: backend\src\Notification\NotificationService\NotificationService.csproj
  Create: backend\src\Notification\NotificationService\Program.cs
  Create: backend\src\Notification\NotificationService\Properties\launchSettings.json
  Create: backend\src\Notification\NotificationService\appsettings.Development.json
  Create: backend\src\Notification\NotificationService\appsettings.json

dotnet sln add backend/src/Notification/NotificationService
```

Nuget packages:

- MassTransit.RabbitMQ: [https://masstransit.io/](https://masstransit.io/)

## Publish to Production Locally

This includes:

- Setup Custom Network for containers
- Give Identity Svc static address
- Add Ingress controller with nginx
- Dockerizing client app
- Adding SSL

The below command displays the default network assigned to containers. We will modify docker-compose to use custom network with ip range. Then assign identity svc static IP.

```powershell
docker network ls
NETWORK ID     NAME                        DRIVER    SCOPE
9df6265aaeb8   bridge                      bridge    local
65a0d8536d8d   carauctionnextnet_default   bridge    local
808066c93d0f   host                        host      local
352caa6f2cd6   none                        null      local
```

[nginxproxy image](hub.docker.com/r/nginxproxy/nginx-proxy)

Update hosts file for reverse proxy services:

```txt
127.0.0.1 id.carauctionnext.com app.carauctionnext.com api.carauctionnext.com
```

Once this is done, then we can start containers and access web-app at:

http://app.carauctionnext.com

### Adding SSL to reverse proxy gateway

[Use OpenSSL to create SSL Cert](https://openssl-library.org/)

#### Create Cert with OpenSSL

[Article 1 on how to Create Self Signed Certificate](https://sockettools.com/kb/creating-certificate-using-openssl/)

[Article 2 on how to Create Self Signed Certificate](https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core/)

##### Step 1: Create the Configuration File

Create carauctionnext.com.conf file in devcerts folder to register these domains:

- DNS.1 = id.carauctionnext.com
- DNS.2 = app.carauctionnext.com
- DNS.3 = api.carauctionnext.com

##### Step 2: Generate the Key and Certificate

run bash command from devcerts folder: `NOTE: modify password before running script!`

```bash
# with password:
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout carauctionnext.com.key -out carauctionnext.com.crt -config carauctionnext.com.conf -passin pass:[password]
# without password:
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout carauctionnext.com.key -out carauctionnext.com.crt -config carauctionnext.com.conf
```

This will create carauctionnext.com.key, carauctionnext.com.crt

##### Step 3: Create the PFX File

Run bash command from nginx folder and enter password:

```bash
openssl pkcs12 -export -out carauctionnext.com.pfx -inkey carauctionnext.com.key -in carauctionnext.com.crt
```

This will create carauctionnext.com.pfx file.

Import certificate into certmgr.exe on your machine.

##### Summary

- Register Domains: Use a domain registrar to register id.carauctionnext.com, app.carauctionnext.com, and api.carauctionnext.com.
- Create Configuration File: Use the provided configuration file to specify the domains.
- Generate Key and Certificate: Use OpenSSL to generate the .key and .crt files.
- Create PFX File: Use OpenSSL to create the .pfx file.

### Issue with SignalR Url in Nextjs

the environment variable in .env.local is not being overridden by web-app in docker-compose file

```txt
.env-local
NEXT_PUBLIC_NOTIFY_URL=http://localhost:6001/notifications

docker-compose.yml > web-app > environment
NEXT_PUBLIC_NOTIFY_URL=http://gateway-svc/notifications
```

[ongoing issue blog post](https://github.com/vercel/next.js/discussions/17641)
