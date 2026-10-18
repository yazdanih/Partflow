FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PartFlow.Api/PartFlow.Api.csproj", "PartFlow.Api/"]
RUN dotnet restore "PartFlow.Api/PartFlow.Api.csproj"

COPY . .
WORKDIR /src/PartFlow.Api
RUN dotnet publish "PartFlow.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "PartFlow.Api.dll"]
