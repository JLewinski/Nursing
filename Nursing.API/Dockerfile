FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5006

ENV ASPNETCORE_URLS=http://+:5006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Nursing.API/Nursing.API.csproj", "Nursing.API/"]
COPY ["Nursing.Core/Nursing.Core.csproj", "Nursing.Core/"]
RUN dotnet restore "Nursing.API\Nursing.API.csproj"1
COPY . .
WORKDIR "/src/Nursing.API"
RUN dotnet restore
RUN dotnet build "Nursing.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Nursing.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nursing.API.dll"]
