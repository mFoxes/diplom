FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG Nuget_Username
ARG Nuget_Password

WORKDIR /src/

COPY ./nuget.config.ci ./nuget.config

COPY ./Domain/Domain.csproj ./Domain/Domain.csproj 
COPY ./LdapConnector/LdapConnector.csproj ./LdapConnector/LdapConnector.csproj
COPY ./GrandmaApi/GrandmaApi.csproj ./GrandmaApi/GrandmaApi.csproj
COPY ./DTO/DTO.csproj ./DTO/DTO.csproj

RUN dotnet restore ./Domain/Domain.csproj 
RUN dotnet restore ./LdapConnector/LdapConnector.csproj
RUN dotnet restore ./GrandmaApi/GrandmaApi.csproj
RUN dotnet restore ./DTO/DTO.csproj

COPY ./Domain ./Domain
COPY ./LdapConnector ./LdapConnector
COPY ./GrandmaApi ./GrandmaApi
COPY ./DTO ./DTO

WORKDIR /src/GrandmaApi
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GrandmaApi.dll"]
