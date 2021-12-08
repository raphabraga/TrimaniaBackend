FROM mcr.microsoft.com/dotnet/sdk:6.0 as BUILD_STAGE
WORKDIR /TrimaniaBackend
COPY *.csproj ./
RUN dotnet restore
COPY . ./
EXPOSE 5044 7064
RUN dotnet dev-certs https --clean && dotnet dev-certs https -t
CMD ["dotnet", "run"]