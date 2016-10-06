FROM microsoft/aspnetcore:1.0.1
RUN curl -sL https://deb.nodesource.com/setup_6.x | bash -
RUN apt-get install -y nodejs
ENV NUGET_XMLDOC_MODE skip
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "mshudx.vote.results.dll"]
ARG source=.
WORKDIR /app
EXPOSE 5000
COPY $source .
