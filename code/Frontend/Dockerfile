#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["code/Frontend/Frontend.csproj", "code/Frontend/"]
COPY ["code/Models/Models.csproj", "code/Models/"]
COPY ["code/CircuitBreaker/CircuitBreaker.csproj", "code/CircuitBreaker/"]
RUN dotnet restore "code/Frontend/Frontend.csproj"
COPY . .
WORKDIR "/src/code/Frontend"
RUN dotnet build "Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Frontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Frontend.dll"]
