FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Sencecon.sln .
COPY src/Sencecon.Domain/Sencecon.Domain.csproj src/Sencecon.Domain/
COPY src/Sencecon.Application/Sencecon.Application.csproj src/Sencecon.Application/
COPY src/Sencecon.Infrastructure/Sencecon.Infrastructure.csproj src/Sencecon.Infrastructure/
COPY src/Sencecon.API/Sencecon.API.csproj src/Sencecon.API/
COPY tests/Sencecon.Application.UnitTests/Sencecon.Application.UnitTests.csproj tests/Sencecon.Application.UnitTests/
RUN dotnet restore Sencecon.sln

COPY . .
RUN dotnet publish src/Sencecon.API/Sencecon.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sencecon.API.dll"]
