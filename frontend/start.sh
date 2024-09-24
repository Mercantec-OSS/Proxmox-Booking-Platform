if [ ! -f "/build/index.js" ]
then
    cd /app
    npm run build
    mv /app/build/* /build
    cp /app/package.json /build
fi

cd /build
node index.js
