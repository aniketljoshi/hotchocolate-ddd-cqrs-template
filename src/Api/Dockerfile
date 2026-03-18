FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["global.json", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Domain/HotChocolateDddCqrsTemplate.Domain.csproj", "src/Domain/"]
COPY ["src/Application/HotChocolateDddCqrsTemplate.Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/HotChocolateDddCqrsTemplate.Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Api/HotChocolateDddCqrsTemplate.Api.csproj", "src/Api/"]

RUN dotnet restore "src/Api/HotChocolateDddCqrsTemplate.Api.csproj"

COPY . .
WORKDIR /src/src/Api
RUN dotnet publish "HotChocolateDddCqrsTemplate.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HotChocolateDddCqrsTemplate.Api.dll"]
