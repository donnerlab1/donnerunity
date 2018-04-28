# Donner

Donner is a Lightning pluding for Unity. It uses grpc to communicate with a local [`lnd`](https://github.com/lightningnetwork/lnd) node.

## Install

### Windows
Should work out of the box, just clone the repo and open with Unity.

### Mac
Copy the files in /Assets/Scripts/runtimes/osx/native to /usr/local/lib/


### Linux
Not tested, could work out of the box.

## Usage
- make sure to run lnd with --no-macaroons flag set
- copy your tls.cert file to /Assets/Resources
- a small wallet example scene is located in Assets/Donner/Examples
- all lightning relevant [`api`](http://api.lightning.community/) calls are implemented.



## Troubleshooting
- execute LndHelper.SetupEnvironmentVariables() in Start() if connection is not working