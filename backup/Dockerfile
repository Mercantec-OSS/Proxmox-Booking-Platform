FROM alpine:3.19
LABEL maintainer="carrergt@gmail.com"

RUN apk update; apk add mariadb-client mariadb-connector-c-dev bash

CMD ["crond","-f"]
