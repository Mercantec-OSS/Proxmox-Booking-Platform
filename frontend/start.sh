if [ ! -d "/build/index.js" ]; then
    cd /app
    npm run build
    mv /app/build/* /build
fi

cd /build
node index.js
