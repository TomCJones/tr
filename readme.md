
<img src="TRstagger192.png" style="display: block; height: 12em; margin: 0 auto;"/>

TR is a public, permissionless, Decentralized Identifier (DID) network based on the ION open source solution that implements the blockchain-agnostic Sidetree protocol on top of Bitcoin (as a 'Layer 2' overlay) to support DIDs/DPKI (Decentralized Public Key Infrastructure) at scale.

**IMPORTANT NOTE:** The majority of TR's code is developed under the blockchain-agnostic Sidetree protocol's repo: https://github.com/decentralized-identity/sidetree, which this project uses internally with the code required to run the protocol on Bitcoin, as the TR network.

## Key Points:

- TR is public and permissionless - the system is decentralized, no company, organization, or group owns/controls the identifiers and DPKI entries in the system, and no one dictates who can participate.
- TR doesn't introduce new tokens/coins - Bitcoin is the only unit of value relevant in the operation of the on-chain aspects of the TR network.
- TR is not a sidechain or consensus system - the network nodes do not require any additional consensus mechanism.

## How does TR work?

By leveraging the blockchain-agnostic Sidetree protocol, TR makes it possible to anchor tens of thousands of DID/DPKI operations on a target chain (in TR's case, Bitcoin) using a single on-chain transaction. The transactions are encoded with a hash that TR nodes use to fetch, store, and replicate the hash-associated DID operation batches via IPFS. The nodes process these batches of operations in accordance with a specific set of deterministic rules that enables them to independently arrive at the correct DPKI state for IDs in the system, without requiring a separate consensus mechanism, blockchain, or sidechain. Nodes can fetch, process, and assemble DID states in parallel, allowing the aggregate capacity of nodes to run at tens of thousands of operations per second.

## Building the project:

Please use the following guide to setup the various services that comprise an TR node: [TR Installation Guide](https://github.com/TomCJones/tr/blob/master/install-guide.md)

## Developer Organizations:

- TR was originally forked from ION, of Sidetree on Bitcoin, which was developed as a part of the [Decentralized Identity Foundation](https://identity.foundation/)

# CRUD Operations

## Create Operation

Creating a DID is registering a did name that is unique and can be claimed by the owner of the private key. To register a mane it must meet certain minimum format rules and be tested to be sure that the name has not be used before.

To register an off-chain DID, the user must submit a JSON body as a HTTP POST request to the registry's endpoint with the following format:

```
 {
   "zonefile": "<zonefile encoding the location of the DDO>",
   "name": "<name, excluding the suffix>",
   "owner_address": "<b58check-encoded address that will own the name>",
 }
```

 ## Read Operation

 ## Update Operation

 ## Deactivate Operation