# maintainer info
FROM node:21
LABEL maintainer="carrergt@gmail.com"

# copy requirements file
WORKDIR /app

# copy app
COPY vmware-frontend start.sh ./

RUN npm i

# start command
CMD ["sh","./start.sh"]
