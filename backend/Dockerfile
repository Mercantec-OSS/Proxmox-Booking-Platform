# maintainer info
FROM debian:11
LABEL maintainer="carrergt@gmail.com"

# install copy and install project dependences
COPY dotnet-install.sh docker-dep.sh start.sh /
RUN sh /docker-dep.sh && sh /dotnet-install.sh

CMD ["bash", "/start.sh"]
