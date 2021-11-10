FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["/src/PizzaItaliano.Services.Products.API/PizzaItaliano.Services.Products.API.csproj", "./PizzaItaliano.Services.Products.API/"]
COPY ["/src/PizzaItaliano.Services.Products.Application/*.csproj", "./PizzaItaliano.Services.Products.Application/"]
COPY ["/src/PizzaItaliano.Services.Products.Core/*.csproj", "./PizzaItaliano.Services.Products.Core/"]
COPY ["/src/PizzaItaliano.Services.Products.Infrastructure/PizzaItaliano.Services.Products.Infrastructure.csproj", "./PizzaItaliano.Services.Products.Infrastructure/"]
RUN dotnet restore "PizzaItaliano.Services.Products.API/PizzaItaliano.Services.Products.API.csproj"

COPY . .
RUN dotnet publish "src/PizzaItaliano.Services.Products.API" -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

ENTRYPOINT ["dotnet", "PizzaItaliano.Services.Products.API.dll"]