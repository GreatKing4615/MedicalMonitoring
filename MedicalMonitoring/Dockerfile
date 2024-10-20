# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MedicalMonitoring/MedicalMonitoring.csproj", "MedicalMonitoring/"]
COPY ["BL/BL.csproj", "BL/"]
COPY ["DAL/DAL.csproj", "DAL/"]
RUN dotnet restore "MedicalMonitoring/MedicalMonitoring.csproj"
COPY . .
WORKDIR "/src/MedicalMonitoring"
RUN dotnet build "MedicalMonitoring.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MedicalMonitoring.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage - use the runtime base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MedicalMonitoring.dll"]
