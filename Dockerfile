FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Thunderbird.sln .
COPY src/Thunderbird.API/Thunderbird.API.csproj src/Thunderbird.API/
COPY src/Thunderbird.Application/Thunderbird.Application.csproj src/Thunderbird.Application/
COPY src/Thunderbird.Domain/Thunderbird.Domain.csproj src/Thunderbird.Domain/
COPY src/Thunderbird.Infrastructure.Caching/Thunderbird.Infrastructure.Caching.csproj src/Thunderbird.Infrastructure.Caching/
COPY src/Thunderbird.Infrastructure.Common/Thunderbird.Infrastructure.Common.csproj src/Thunderbird.Infrastructure.Common/
COPY src/Thunderbird.Infrastructure.IOC/Thunderbird.Infrastructure.IOC.csproj src/Thunderbird.Infrastructure.IOC/
COPY src/Thunderbird.Infrastructure.Logging/Thunderbird.Infrastructure.Logging.csproj src/Thunderbird.Infrastructure.Logging/
COPY src/Thunderbird.Infrastructure.Persistance/Thunderbird.Infrastructure.Persistance.csproj src/Thunderbird.Infrastructure.Persistance/
RUN dotnet restore src/Thunderbird.API/Thunderbird.API.csproj

COPY src/ src/
RUN dotnet publish src/Thunderbird.API/Thunderbird.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app .

USER app
ENTRYPOINT ["dotnet", "Thunderbird.API.dll"]
