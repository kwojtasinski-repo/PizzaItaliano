FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY src/**/*.csproj .
RUN dotnet restore "PizzaItaliano.Services.Products.API.csproj"

COPY . .
RUN dotnet publish src/PizzaItaliano.Services.Products.API -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker

RUN groupadd -g 10001 app && \
    useradd -u 10000 -g app app \
    && chown -R app:app ./
RUN chmod -R 0500 ./
RUN chmod -R 0700 *.dll # if the dll's are not writeable, it won't start
USER app:app

ENTRYPOINT ["dotnet", "PizzaItaliano.Services.Products.API.dll"]