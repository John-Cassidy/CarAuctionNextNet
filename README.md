# CarAuctionNextNet

How to build a microservices based app using .Net, NextJS, IdentityServer, RabbitMQ running on Docker and Kubernetes

## dotnet tools

```powershell
dotnet tool list -g
dotnet tool install --global [TOOL_NAME] --version x.x.x
dotnet tool update --global [TOOL_NAME] #updates to latest version
```

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
