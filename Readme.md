# Donner

Donner is a Lightning plugin for Unity. It uses grpc to communicate with a local [`lnd`](https://github.com/lightningnetwork/lnd) node.

## Install

### Windows
Should work out of the box, just clone the repo and open with Unity.

### Mac
Copy the files in /Assets/Scripts/runtimes/osx/native to /usr/local/lib/


### Linux
Not tested, could work out of the box.

## Usage
- copy your tls.cert and admin.macaroon files to /Assets/Resources
- a small wallet example scene is located in Assets/Donner/Examples
- all lightning relevant [`api`](http://api.lightning.community/) calls are implemented.

##Examples

#SimpleWallet
- lnd wallet made with unity
- point the Simple Lnd Wallet Scripts of Donner Gameobject to your local or remote lnd node (hostname and port). and copy you tls.cert and admin.macaroon inside you Resources folder

#HttpServerPayments
- creates a http webserver that creates invoices. if the invoices are settled the weather changes
- goto localhost:8080/weather? to see instruction

#NetworkPaymentsLnd
- buggy atm

## Troubleshooting
- run LndHelper.SetupEnvironmentVariables() in Start() if connection is not working