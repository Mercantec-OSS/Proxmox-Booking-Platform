# maintainer info
FROM debian:11 AS builder
LABEL maintainer="carrergt@gmail.com"

# install copy and install project dependences
COPY dotnet-install.sh powershell_install.sh powercli_install.ps1 docker-dep.sh /

RUN sh /docker-dep.sh && sh /dotnet-install.sh && sh /powershell_install.sh && pwsh /powercli_install.ps1

FROM builder AS publish

# copy dependences
WORKDIR /app

COPY app ./

RUN dotnet publish --sc -o /build

CMD ["/build/app"]
