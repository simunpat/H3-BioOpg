FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src
COPY . .
ENV ASPNETCORE_URLS=http://+:5099

# Repo-relative path since we mount the whole repo at /src
RUN dotnet restore Backend/BiografWeb.Api/BiografWeb.Api.csproj
CMD ["dotnet","run","--project","Backend/BiografWeb.Api/BiografWeb.Api.csproj","--no-launch-profile"]