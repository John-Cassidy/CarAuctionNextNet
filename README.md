# CarAuctionNextNet

How to build a microservices based app using .Net, NextJS, IdentityServer, RabbitMQ running on Docker and Kubernetes

## Dockerfile

Build AuctionService image: open prompt in solution folder and run the following command:

```powershell
# docker build --force-rm -t auctionservice:dev --target base --build-arg "BUILD_CONFIGURATION=Debug" -f "./backend/src/Auction/AuctionService/Dockerfile" .
docker build --force-rm -t auctionservice:latest -f "./backend/src/Auction/AuctionService/Dockerfile" .

docker run -d -p 8080:8080 --name auctionservice auctionservice
```
