# çalýþacaðý ortam belirleniyor
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

#derlenecek imajý hazýrlayalým
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Microservice.ProductWebAPI/Microservice.ProductWebAPI.csproj", "Microservice.ProductWebAPI/"]
RUN dotnet restore "./Microservice.ProductWebAPI/Microservice.ProductWebAPI.csproj"
COPY . .
WORKDIR /src/Microservice.ProductWebAPI
RUN dotnet build "./Microservice.ProductWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

#publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Microservice.ProductWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish

#final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservice.ProductWebAPI.dll"]