# maintainer info
FROM debian:11
LABEL maintainer="carrergt@gmail.com"

# install copy and install project dependences
COPY dotnet-install.sh start.sh /
RUN sh /dotnet-install.sh

CMD ["bash", "/start.sh"]
