FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GLMS.sln ./
COPY GLMS.Api/GLMS.Api.csproj GLMS.Api/
COPY GLMS/GLMS.csproj GLMS/
COPY GLMS.Tests/GLMS.Tests.csproj GLMS.Tests/

RUN dotnet restore GLMS.Api/GLMS.Api.csproj

COPY . .
RUN dotnet publish GLMS.Api/GLMS.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

#make sure the pdf directory is there
RUN mkdir -p /app/wwwroot/uploads/contracts

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GLMS.Api.dll"]