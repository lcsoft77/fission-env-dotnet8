#!/bin/bash

# Nome dell'immagine e tag
IMAGE_NAME="lcsoft/net8_env"
TAG="latest"

# Costruisci l'immagine Docker
docker build -t $IMAGE_NAME:$TAG .

# Fai il push dell'immagine sul repository
docker push $IMAGE_NAME:$TAG

# Stampa un messaggio di successo
echo "Immagine Docker $IMAGE_NAME:$TAG costruita e pushata con successo su 192.168.1.73"