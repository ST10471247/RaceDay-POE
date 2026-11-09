FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RaceDay.MVC.csproj", "./"]
RUN dotnet restore "RaceDay.MVC.csproj"
COPY . .
RUN dotnet publish "RaceDay.MVC.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "RaceDay.MVC.dll"]