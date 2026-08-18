FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY . .

RUN dotnet restore ServerUNO/ServerUNO.csproj
RUN dotnet build ServerUNO/ServerUNO.csproj -c Release
EXPOSE 5000
CMD ["dotnet", "run", "--no-build", "-c", "Release", "--project", "ServerUNO/ServerUNO.csproj"]