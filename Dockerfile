# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
WORKDIR /src
COPY ["realEstateManagementAPI/realEstateManagementAPI.csproj", "realEstateManagementAPI/"]
COPY ["realEstateManagementBusinessLayer/realEstateManagementBusinessLayer.csproj", "realEstateManagementBusinessLayer/"]
COPY ["realEstateManagementDataLayer/realEstateManagementDataLayer.csproj", "realEstateManagementDataLayer/"]
COPY ["realEstateManagementEntities/realEstateManagementEntities.csproj", "realEstateManagementEntities/"]
RUN dotnet restore "realEstateManagementAPI/realEstateManagementAPI.csproj"
COPY . .
WORKDIR "/src/realEstateManagementAPI"
RUN dotnet build "realEstateManagementAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "realEstateManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim-amd64 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
COPY realEstateManagementAPI/mycert.crt .
COPY realEstateManagementAPI/mycert.pfx .
COPY realEstateManagementAPI/mykey.key .
ENTRYPOINT ["dotnet", "realEstateManagementAPI.dll"]
