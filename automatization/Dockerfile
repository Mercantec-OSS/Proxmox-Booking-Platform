# maintainer info
FROM debian:11
LABEL maintainer="carrergt@gmail.com"

# install copy and install project dependences
COPY dotnet-install.sh powershell_install.sh powercli_install.ps1 docker-dep.sh start.sh /
RUN sh /docker-dep.sh && sh /dotnet-install.sh && sh /powershell_install.sh && pwsh /powercli_install.ps1

CMD ["bash", "/start.sh"]
