FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["/src/PizzaItaliano.Services.Orders.API/*.csproj", "./PizzaItaliano.Services.Orders.API/"]
COPY ["/src/PizzaItaliano.Services.Orders.Application/*.csproj", "./PizzaItaliano.Services.Orders.Application/"]
COPY ["/src/PizzaItaliano.Services.Orders.Core/*.csproj", "./PizzaItaliano.Services.Orders.Core/"]
COPY ["/src/PizzaItaliano.Services.Orders.Infrastructure/*.csproj", "./PizzaItaliano.Services.Orders.Infrastructure/"]
RUN dotnet restore "PizzaItaliano.Services.Orders.API/PizzaItaliano.Services.Orders.API.csproj"

COPY . .
RUN dotnet publish "src/PizzaItaliano.Services.Orders.API" -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

ENTRYPOINT ["dotnet", "PizzaItaliano.Services.Orders.API.dll"]