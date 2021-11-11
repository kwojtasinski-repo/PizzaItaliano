FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY src/**/*.csproj .
RUN dotnet restore "PizzaItaliano.APIGateway.csproj"

COPY . .
RUN dotnet publish src/PizzaItaliano.APIGateway -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY src/PizzaItaliano.APIGateway/ntrada.docker.yml .
COPY src/PizzaItaliano.APIGateway/ntrada-async.docker.yml .
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENV NTRADA_CONFIG ntrada.docker

ENTRYPOINT ["dotnet", "PizzaItaliano.APIGateway.dll"]