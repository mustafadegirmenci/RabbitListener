FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RabbitListener.Worker/RabbitListener.Worker.csproj", "RabbitListener.Worker/"]
RUN dotnet restore "RabbitListener.Worker/RabbitListener.Worker.csproj"
COPY . .
WORKDIR "/src/RabbitListener.Worker"
RUN dotnet build "RabbitListener.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitListener.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitListener.Worker.dll"]
