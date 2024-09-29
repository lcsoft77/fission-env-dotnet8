#!/bin/bash


# Nome dell'immagine e tag
IMAGE_NAME="lcsoft/net8_env"
TAG="22"

# Costruisci l'immagine Docker
docker build -t $IMAGE_NAME:$TAG .

# Fai il push dell'immagine sul repository
docker push $IMAGE_NAME:$TAG

# Stampa un messaggio di successo
echo "Immagine Docker $IMAGE_NAME:$TAG costruita e pushata con successo su 192.168.1.73"


docker image rm $IMAGE_NAME:$TAG

fission function delete --name dotnet8test

fission env delete --name dotnet8

fission env create --name dotnet8 --image $IMAGE_NAME:$TAG --poolsize 1

fission function create --name dotnet8test --env dotnet8 --code TestExternalLibrary/bin/Debug/net8.0/TestExternalLibrary.zip --entrypoint "TestExternalLibrary.TestExternalLibrary.Class1"