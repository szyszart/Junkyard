#!/bin/bash
# Generates sprite sheet from raw data.
TILES_PER_ROW=5

if (($# != 1)); then
    echo "Usage: $0 directory_path"
    exit 1
fi;

WINPATH=$(cygpath -pwm $1)
TMPFILE=`mktemp log.XXXXXXXXXX` || exit 1	
echo "$WINPATH"
find "$1" -regex "^.*/[0-9].*\.png" -printf "${WINPATH}/%f\\0${WINPATH}/c%f\\0" | xargs -0 -L 2 -r convert -trim -verbose &> $TMPFILE
montage -background none -tile ${TILES_PER_ROW}x -gravity NorthWest -geometry "100%+0+0" "$WINPATH/c*.png" "$WINPATH/out.png" 
python infogen.py $TMPFILE $TILES_PER_ROW > "$WINPATH/out.lua"
rm $TMPFILE
