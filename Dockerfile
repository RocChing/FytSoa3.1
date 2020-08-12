#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 9000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FytSoa.Api/FytSoa.Api.csproj", "FytSoa.Api/"]
COPY ["FytSoa.Common/FytSoa.Common.csproj", "FytSoa.Common/"]
COPY ["FytSoa.Service/FytSoa.Service.csproj", "FytSoa.Service/"]
COPY ["FytSoa.Core/FytSoa.Core.csproj", "FytSoa.Core/"]
COPY ["FytSoa.Extensions/FytSoa.Extensions.csproj", "FytSoa.Extensions/"]
RUN dotnet restore "FytSoa.Api/FytSoa.Api.csproj"
COPY . .
WORKDIR "/src/FytSoa.Api"
RUN dotnet build "FytSoa.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FytSoa.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FytSoa.Api.dll"]