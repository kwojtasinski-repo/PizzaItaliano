FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["/src/PizzaItaliano.Services.Releases.API/*.csproj", "./PizzaItaliano.Services.Releases.API/"]
COPY ["/src/PizzaItaliano.Services.Releases.Application/*.csproj", "./PizzaItaliano.Services.Releases.Application/"]
COPY ["/src/PizzaItaliano.Services.Releases.Core/*.csproj", "./PizzaItaliano.Services.Releases.Core/"]
COPY ["/src/PizzaItaliano.Services.Releases.Infrastructure/*.csproj", "./PizzaItaliano.Services.Releases.Infrastructure/"]
RUN dotnet restore "PizzaItaliano.Services.Releases.API/PizzaItaliano.Services.Releases.API.csproj"

COPY . .
RUN dotnet publish "src/PizzaItaliano.Services.Releases.API" -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

ENTRYPOINT ["dotnet", "PizzaItaliano.Services.Releases.API.dll"]