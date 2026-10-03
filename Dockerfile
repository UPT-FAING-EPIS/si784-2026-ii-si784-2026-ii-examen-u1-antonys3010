# syntax=docker/dockerfile:1
FROM node:22-alpine AS frontend
WORKDIR /src/frontend
COPY frontend/package*.json ./
RUN npm install
COPY frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/ backend/
RUN dotnet restore backend/FlightReservation.Api.csproj
RUN dotnet publish backend/FlightReservation.Api.csproj -c Release -o /app/publish --no-restore
COPY --from=frontend /src/frontend/dist/ /app/publish/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet","FlightReservation.Api.dll"]
