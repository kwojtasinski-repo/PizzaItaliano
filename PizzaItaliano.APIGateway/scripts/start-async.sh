#!/bin/bash
export ASPNETCORE_ENVIRONMENT=Development
export NTRADA_CONFIG=ntrada-async
cd ..
cd src/PizzaItaliano.APIGateway
dotnet run
