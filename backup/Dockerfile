FROM alpine:3.19
LABEL maintainer="carrergt@gmail.com"

COPY start.sh restore_backup.sh /
RUN apk update; apk add mariadb-client mariadb-connector-c-dev bash

CMD ["bash","/start.sh"]
