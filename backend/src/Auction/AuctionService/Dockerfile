#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/src/Auction/AuctionService/AuctionService.csproj", "backend/src/Auction/AuctionService/"]
COPY ["backend/src/Infrastructure/Contracts/Contracts.csproj", "backend/src/Infrastructure/Contracts/"]
RUN dotnet restore "./backend/src/Auction/AuctionService/AuctionService.csproj"
COPY . .
WORKDIR "/src/backend/src/Auction/AuctionService"
RUN dotnet build "./AuctionService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuctionService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuctionService.dll"]