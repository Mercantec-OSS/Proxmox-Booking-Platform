CURRENT_DATE=$(date +%d-%m-%Y_%H-%M-%S)
FILE_NAME="$CURRENT_DATE"_app.tar.gz
CURRENT_PATH=$(pwd)
CURRENT_FORLDER=$(basename $CURRENT_PATH)

cd ..
tar --exclude=.env --exclude=db/storage -czf $FILE_NAME $CURRENT_FORLDER
