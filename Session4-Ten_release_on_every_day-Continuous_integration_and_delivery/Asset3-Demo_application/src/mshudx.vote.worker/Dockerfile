FROM microsoft/dotnet:1.0.1-core
ENTRYPOINT ["dotnet", "mshudx.vote.worker.dll"]
ARG source=.
WORKDIR /app
COPY $source .
