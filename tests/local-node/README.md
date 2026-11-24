# Local TRON node

`private.conf` is java-tron's default config turned into a one-witness private chain:
- genesis funds `THe8MeboLUbvi1TYntzPKK8SsfcCiJCZ19` (private key in `tests/Nexutron.Tests/TestVectors.cs`, test only),
- the witness is `TBDCyrZ1hT1PDDFf2yRABwPrFica5qqPUX`, which the node uses to produce blocks,
- constant calls and contract creation are enabled, discovery and seed nodes are disabled.

`Token.sol` is a minimal TRC20 used by the tests; `compile.js` rebuilds
`tests/Nexutron.LocalNode.Tests/Token.json` with solc-js 0.8.19 (`evmVersion: istanbul`):

```bash
npm i solc@0.8.19 && node compile.js Token.sol ../Nexutron.LocalNode.Tests/Token.json
```

Run the tests:

```bash
./tests/local-node/start.sh
NEXUTRON_LOCAL_NODE=localhost dotnet test tests/Nexutron.LocalNode.Tests
docker rm -f nexutron-tron-node
```
