folder="/app/build"
if [ ! -d "$folder" ]; then
    npm run build
fi

node index.js
