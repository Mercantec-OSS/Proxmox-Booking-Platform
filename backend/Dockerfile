# maintainer info
FROM debian:11 AS dependencies
LABEL maintainer="carrergt@gmail.com"

# install copy and install project dependences
COPY dotnet-install.sh docker-dep.sh /
RUN sh /docker-dep.sh && sh /dotnet-install.sh

# build project
FROM dependencies AS build
WORKDIR /app
COPY ./app /app
RUN mkdir /build
RUN dotnet publish --sc -o /build

# Run project
WORKDIR /build
CMD ["/build/app"]
