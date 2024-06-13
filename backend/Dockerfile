# maintainer info
FROM debian:11 AS builder
LABEL maintainer="carrergt@gmail.com"

# install copy and install project dependences
COPY dotnet-install.sh /

RUN sh /dotnet-install.sh

FROM builder AS publish

# copy dependences
WORKDIR /app

COPY vmware-backend/vmware_backend ./

RUN dotnet publish --sc -o /build

CMD ["/build/app", "--urls", "http://0.0.0.0:5000"]
