#!/bin/sh

# Veritabanı güncellemesini gerçekleştir
dotnet ef database update --project /src/realEstateManagementDataLayer/realEstateManagementDataLayer.csproj --startup-project /src/realEstateManagementAPI/realEstateManagementAPI.csproj

# Uygulamayı başlat
exec dotnet realEstateManagementAPI.dll
