# maintainer info
FROM node:23 AS dependencies
LABEL maintainer="carrergt@gmail.com"

COPY ./app /app

WORKDIR /app
RUN npm i && npm run build

# start command
CMD ["node","/app/build/index.js"]
