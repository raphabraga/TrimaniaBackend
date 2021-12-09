FROM mcr.microsoft.com/dotnet/sdk:6.0 as BUILD_STAGE
WORKDIR /Backend
COPY *.csproj ./
RUN dotnet restore
COPY . ./
EXPOSE 5044 5045
CMD ["dotnet", "run"]