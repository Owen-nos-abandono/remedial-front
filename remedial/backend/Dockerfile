FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el repo
COPY . .

# Entramos a la carpeta según tu captura de pantalla
WORKDIR "/src/ArroyoSeco-BackEnd-main/arroyoSeco"

# LISTAMOS LOS ARCHIVOS (Para ver si el .csproj se llama arroyoSeco.API.csproj o arroyoSeco.csproj)
RUN ls -la

# INTENTAMOS COMPILAR CON VERBOSIDAD MÍNIMA PARA VER EL ERROR REAL
RUN dotnet build "arroyoSeco.API.csproj" -c Release

# SI EL BUILD PASA, ENTONCES PUBLICAMOS
RUN dotnet publish "arroyoSeco.API.csproj" -c Release -o /app --no-restore
# -------------------------

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "arroyoSeco.API.dll"]
