FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

RUN apt-get update && apt-get install -y curl iputils-ping && rm -rf /var/lib/apt/lists/*

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Mini_MES_API.dll"]