FROM microsoft/dotnet:2.2-sdk
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
RUN chmod +x ./entrypoint.sh