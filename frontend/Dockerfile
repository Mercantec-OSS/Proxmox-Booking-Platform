# maintainer info
FROM node:21
LABEL maintainer="carrergt@gmail.com"

COPY app start.sh /

WORKDIR /app
RUN npm i

# start command
CMD ["sh","/start.sh"]
