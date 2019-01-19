FROM alpine/git as git
WORKDIR /app
RUN  git clone https://github.com/midianok/IpCameraClient.git

FROM microsoft/dotnet:2.2-sdk-alpine as build
WORKDIR /app
COPY --from=git /app .
RUN dotnet restore ./IpCameraClient/IpCameraClient.ConsoleFacade
RUN dotnet publish ./IpCameraClient/IpCameraClient.ConsoleFacade -c Release -o out

FROM microsoft/dotnet:2.2-runtime-alpine
WORKDIR /app
COPY --from=build /app/IpCameraClient/IpCameraClient.ConsoleFacade/out .
COPY appsettings.json .

ENTRYPOINT  [ "dotnet",  "IpCameraClient.ConsoleFacade.dll"]