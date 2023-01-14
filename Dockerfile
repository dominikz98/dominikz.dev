FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /work
EXPOSE 80
COPY . .
RUN dotnet restore ./src/dominikz.Application/dominikz.Application.csproj
RUN dotnet publish ./src/dominikz.Application/dominikz.Application.csproj -c Release -o /artifacts --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as serve
WORKDIR /srv
COPY --from=build ./artifacts .

ENTRYPOINT ["dotnet", "dominikz.Application.dll"]
