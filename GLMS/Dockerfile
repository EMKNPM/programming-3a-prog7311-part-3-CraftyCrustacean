FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GLMS.sln ./
COPY GLMS/GLMS.csproj GLMS/
COPY GLMS.Api/GLMS.Api.csproj GLMS.Api/
COPY GLMS.Tests/GLMS.Tests.csproj GLMS.Tests/

RUN dotnet restore GLMS/GLMS.csproj

COPY . .
RUN dotnet publish GLMS/GLMS.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GLMS.dll"]