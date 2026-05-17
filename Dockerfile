FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NutriEval.API/NutriEval.API.csproj NutriEval.API/
RUN dotnet restore NutriEval.API/NutriEval.API.csproj

COPY . .
RUN dotnet publish NutriEval.API/NutriEval.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NutriEval.API.dll"]
