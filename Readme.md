# DonnerUnity

## Introduction
DonnerUnity is a [Lightning](https://lightning.network/) based payment plugin for the Unity game engine. It enables near-instant, scalable and low cost microtransactions using sound and permissionless money. The plugin can replace currently used payments systems and enables completly new game mechanics. It allows developers to provide players with a sense of pride and accomplishment.

### Comparison

| Service | fee | minimum amount | maximum amount | personal data required | trusted third party | 
:---:|:---:|:---:|:---:|:---:|:---:
**DonnerUnity** | 0% | $0.00008 * | $137 billion *|no | no
**Steamworks** | 30% | $0.01 | $2000 in 24h |yes | yes
**App Store** | 30% | $0.99 | $999.99 | yes  | yes

*: assuming 8000$ USD/BTC


## Installation

### Prerequisites

- Unity 2017.1 or newer
- a locally or remote running [lnd](https://github.com/lightningnetwork/lnd) node (see [installation instructions](docs/INSTALL.md))

### Importing

- clone the repository or download the binary
- make sure your project is set to .NET 4.6 runtime
- import the donner folder or assetbundle

### Setup

- copy your tls.cert and admin.macaroon to Assets/Resources/
- edit the donner.conf file in Assets/Resources/ to your liking

### Building

- after building your project copy your tls.cert, admin.macaroon and donner.conf to the _Data/Resources folder
- copy the files grpc_csharp_ext.x86.dll and grpc_csharp_ext.x64.dll from _Data/Plugins in your _Data/Managed folder
- or run the editor items under "Donner"


## Usage

- create a class that inherits from LndRpcBridge and create your own custom logic
- it is best explained by looking through the example scenes

## Example scenes

### BasicWallet

- simple lightning wallet with basic functionality
- look at Donner Gameobject and SimpleLndWallet.cs for logic

### lnplaysController

- controller for [lnplays](https://lnplays.com)(currently down)
- look at Donner Gameobject and LnPlaysController.cs for logic

### DonnerWeatherTwitch
- listens to your twitch chat for commands !rain !fire !channel and sends invoices to the chat
- if a invoice is paid the weather changes
- change Oauth Nick Name and Channel Name of TwitchIRC gameobject to yours
- look at Donner Gameobject, WeatherLndClient.cs and TwitchLnd.cs for logic

### DonnerWeatherWeb
- same functionality as the twitch version
- creates webserver that sends invoices
- goto localhost:8079/weather? to see instructions
- look at Donner Gameobject, WeatherLndClient.cs and PaymentsHttpServer.cs for logic


