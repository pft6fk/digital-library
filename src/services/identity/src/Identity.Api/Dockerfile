FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Identity.Domain/Identity.Domain.csproj src/Identity.Domain/
COPY src/Identity.Application/Identity.Application.csproj src/Identity.Application/
COPY src/Identity.Infrastructure/Identity.Infrastructure.csproj src/Identity.Infrastructure/
COPY src/Identity.Api/Identity.Api.csproj src/Identity.Api/

RUN dotnet restore src/Identity.Api/Identity.Api.csproj

COPY src/ src/

RUN dotnet publish src/Identity.Api/Identity.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Identity.Api.dll"]
