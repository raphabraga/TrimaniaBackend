FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build_stage
WORKDIR /Trimania
COPY ./*.sln ./
COPY ./Backend/*.csproj ./Backend/
COPY ./BackendTest/*.csproj ./BackendTest/
RUN dotnet restore
COPY . .
RUN dotnet build

FROM build_stage AS test_runner
WORKDIR /Trimania/BackendTest
CMD ["dotnet", "test", "--logger:trx"]

FROM build_stage AS unit_tests
WORKDIR /Trimania/BackendTest
RUN dotnet test --logger:trx

FROM build_stage AS publish
WORKDIR /Trimania/Backend
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS RUN_TIME
WORKDIR /Trimania
COPY --from=publish Trimania/Backend/out .
EXPOSE 5044
ENV ASPNETCORE_URLS=http://+:5044
ENTRYPOINT ["dotnet", "Backend.dll"]