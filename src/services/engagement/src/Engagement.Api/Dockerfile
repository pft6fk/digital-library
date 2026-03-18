FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Engagement.Domain/Engagement.Domain.csproj src/Engagement.Domain/
COPY src/Engagement.Application/Engagement.Application.csproj src/Engagement.Application/
COPY src/Engagement.Infrastructure/Engagement.Infrastructure.csproj src/Engagement.Infrastructure/
COPY src/Engagement.Api/Engagement.Api.csproj src/Engagement.Api/

RUN dotnet restore src/Engagement.Api/Engagement.Api.csproj

COPY src/ src/

RUN dotnet publish src/Engagement.Api/Engagement.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Engagement.Api.dll"]
