#!/bin/sh

openssl version > /dev/null 2>&1

if [ $? -ne 0 ]; then
    echo "openssl was not found" >&2
    exit $?
fi

private_key="private_key.pem"
public_key="public_key.pem"

if [ -e "$private_key" ]; then
    echo "private key already exists"
    exit 0
fi

openssl genpkey -algorithm RSA -out "$private_key" -pkeyopt rsa_keygen_bits:2048
openssl rsa -pubout -in "$private_key" -out "$public_key"
cat "$private_key" | tr -d '\n' > private_key_flat.txt 
cat "$public_key" | tr -d '\n' > public_key_flat.txt 