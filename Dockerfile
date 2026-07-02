FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TestProvinciaApi/TestProvinciaApi.csproj TestProvinciaApi/
COPY TestProvincia.Application/TestProvincia.Application.csproj TestProvincia.Application/
COPY TestProvincia.Domain/TestProvincia.Domain.csproj TestProvincia.Domain/
COPY TestProvincia.Infrastructure/TestProvincia.Infrastructure.csproj TestProvincia.Infrastructure/
COPY TestProvincia.Shared/TestProvincia.Shared.csproj TestProvincia.Shared/
RUN dotnet restore TestProvinciaApi/TestProvinciaApi.csproj

COPY . .
WORKDIR /src/TestProvinciaApi
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TestProvinciaApi.dll"]
