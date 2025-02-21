FROM alpine:3.19
LABEL maintainer="carrergt@gmail.com"

COPY start.sh restore_backup.sh /
COPY ./backup_script.sh /etc/periodic/daily/backup_script.sh
RUN apk update; apk add mariadb-client mariadb-connector-c-dev bash

CMD ["bash","/start.sh"]
