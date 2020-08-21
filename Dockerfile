FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /generator

COPY api/api.csproj ./api/
RUN dotnet restore api/api.csproj
COPY tests/tests.csproj ./tests/
RUN dotnet restore tests/tests.csproj

#RUN ls -alR (Show files copied to the container)
#copy src
COPY . .

#tests
ENV TEAMCITY_PROJECT_NAME=fake
RUN dotnet test tests/tests.csproj

#publish
RUN dotnet publish api/api.csproj -o /publish

#runtime stage
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build-env /publish /publish
WORKDIR /publish

ENV ASPNETCORE_URLS http://+:80
ENTRYPOINT ["dotnet","api.dll"]