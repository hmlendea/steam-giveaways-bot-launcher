#!/bin/bash

APP_NAME="steam-giveaways-bot-launcher"
VERSION_MAJOR=$(date +"%y.%j")
VERSION_MINOR="$1"
RELEASE_DIR_RELATIVE="bin/Release"
PUBLISH_DIR_RELATIVE="${RELEASE_DIR_RELATIVE}/publish-script-output"
RELEASE_DIR="$(pwd)/$RELEASE_DIR_RELATIVE"
PUBLISH_DIR="$(pwd)/$PUBLISH_DIR_RELATIVE"

if [ -z $VERSION_MINOR ]; then
    VERSION_MINOR="0"
fi

VERSION="$VERSION_MAJOR.$VERSION_MINOR"

function package {
    ARCH="$1"

    OUTPUT_DIR="$PUBLISH_DIR/$ARCH"
    OUTPUT_FILE="$RELEASE_DIR/${APP_NAME}_${VERSION}_${ARCH}.zip"

    echo "Packaging \"$OUTPUT_DIR\" to \"$OUTPUT_FILE\""

    if [ -f "$OUTPUT_FILE" ]; then
        rm "$OUTPUT_FILE"
    fi

    cd "$OUTPUT_DIR"
    zip -q -9 -r "$OUTPUT_FILE" .
    cd -
}

function dotnet-pub {
    ARCH="$1"
    OUTPUT_DIR="$PUBLISH_DIR_RELATIVE/$ARCH"

    dotnet publish -c Release -r "$ARCH" -o "$OUTPUT_DIR" --self-contained=true /p:TrimUnusedDependencies=true /p:LinkDuringPublish=true
}

function prepare {
    echo "Adding the temporary NuGet packages"
    dotnet add package Microsoft.Packaging.Tools.Trimming --version 1.1.0-preview1-26619-01
    #dotnet add package ILLink.Tasks --version 0.1.5-preview-1841731 --source https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
}

function cleanup {
    echo "Removing the temporary NuGet packages"
    dotnet remove package Microsoft.Packaging.Tools.Trimming
    #dotnet remove package ILLink.Tasks    

    echo "Cleaning build output"
    rm -rf "$PUBLISH_DIR"
}

prepare

dotnet-pub win-x64
package win-x64

dotnet-pub linux-x64
package linux-x64

dotnet-pub linux-arm
package linux-arm

cleanup
