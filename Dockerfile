# Utilizza l'immagine base di ASP.NET Core 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Utilizza l'immagine SDK di .NET 8 per il build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
#COPY Fission.DotNet/Fission.DotNet.csproj Fission.DotNet
#COPY Fission.DotNet.Common/Fission.DotNet.Common.csproj Fission.DotNet.Common
RUN dotnet restore Fission.DotNet/Fission.DotNet.csproj

# Copia il codice sorgente e compila
#COPY . .
RUN dotnet publish Fission.DotNet/Fission.DotNet.csproj -c Release -o /app

# Immagine finale
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Fission.DotNet.dll"]
