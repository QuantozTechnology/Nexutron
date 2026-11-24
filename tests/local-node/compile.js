const solc = require('solc'), fs = require('fs');
const input = { language: 'Solidity', sources: { 'Token.sol': { content: fs.readFileSync(process.argv[2], 'utf8') } },
  settings: { evmVersion: 'istanbul', optimizer: { enabled: true, runs: 200 }, outputSelection: { '*': { '*': ['abi', 'evm.bytecode.object'] } } } };
const out = JSON.parse(solc.compile(JSON.stringify(input)));
if (out.errors?.some(e => e.severity === 'error')) { console.error(out.errors); process.exit(1); }
const c = out.contracts['Token.sol'].Token;
fs.writeFileSync(process.argv[3], JSON.stringify({ abi: c.abi, bytecode: c.evm.bytecode.object }));
console.log('bytecode bytes:', c.evm.bytecode.object.length / 2);
