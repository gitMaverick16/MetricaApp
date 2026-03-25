FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/OrdersApp.Api/OrdersApp.Api.csproj", "src/OrdersApp.Api/"]
COPY ["src/OrdersApp.Application/OrdersApp.Application.csproj", "src/OrdersApp.Application/"]
COPY ["src/OrdersApp.Domain/OrdersApp.Domain.csproj", "src/OrdersApp.Domain/"]
COPY ["src/OrdersApp.Infrastructure/OrdersApp.Infrastructure.csproj", "src/OrdersApp.Infrastructure/"]

RUN dotnet restore "src/OrdersApp.Api/OrdersApp.Api.csproj"

COPY src ./src
RUN dotnet publish "src/OrdersApp.Api/OrdersApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrdersApp.Api.dll"]
