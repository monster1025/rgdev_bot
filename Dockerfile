FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ADD ./src /src
WORKDIR /src

# Restore
RUN dotnet restore RgDevBot.sln
RUN dotnet build RgDevBot.sln -c Release -o /app
RUN dotnet test RgDevBot.sln -c Release
RUN dotnet publish RgDevBot.sln -c Release -o /app

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "RgDevBot.dll"]
