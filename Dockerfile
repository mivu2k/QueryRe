FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY QueryRe.Core/QueryRe.Core.csproj QueryRe.Core/
COPY QueryRe.Data/QueryRe.Data.csproj QueryRe.Data/
COPY QueryRe.Web/QueryRe.Web.csproj QueryRe.Web/
RUN dotnet restore QueryRe.Web/QueryRe.Web.csproj

COPY . .
RUN dotnet publish QueryRe.Web/QueryRe.Web.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "QueryRe.Web.dll"]
