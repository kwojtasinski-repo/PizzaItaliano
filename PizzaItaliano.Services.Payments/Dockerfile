FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["/src/PizzaItaliano.Services.Payments.API/*.csproj", "./PizzaItaliano.Services.Payments.API/"]
COPY ["/src/PizzaItaliano.Services.Payments.Application/*.csproj", "./PizzaItaliano.Services.Payments.Application/"]
COPY ["/src/PizzaItaliano.Services.Payments.Core/*.csproj", "./PizzaItaliano.Services.Payments.Core/"]
COPY ["/src/PizzaItaliano.Services.Payments.Infrastructure/*.csproj", "./PizzaItaliano.Services.Payments.Infrastructure/"]
RUN dotnet restore "PizzaItaliano.Services.Payments.API/PizzaItaliano.Services.Payments.API.csproj"

COPY . .
RUN dotnet publish "src/PizzaItaliano.Services.Payments.API" -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

ENTRYPOINT ["dotnet", "PizzaItaliano.Services.Payments.API.dll"]