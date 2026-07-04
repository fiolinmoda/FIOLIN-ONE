FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/Directory.Build.props backend/
COPY backend/src/FiolinOne.Domain/FiolinOne.Domain.csproj backend/src/FiolinOne.Domain/
COPY backend/src/FiolinOne.Application/FiolinOne.Application.csproj backend/src/FiolinOne.Application/
COPY backend/src/FiolinOne.Infrastructure/FiolinOne.Infrastructure.csproj backend/src/FiolinOne.Infrastructure/
COPY backend/src/FiolinOne.Api/FiolinOne.Api.csproj backend/src/FiolinOne.Api/
RUN dotnet restore backend/src/FiolinOne.Api/FiolinOne.Api.csproj

COPY backend backend
RUN dotnet publish backend/src/FiolinOne.Api/FiolinOne.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FiolinOne.Api.dll"]
