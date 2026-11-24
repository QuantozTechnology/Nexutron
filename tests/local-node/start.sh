#!/usr/bin/env bash
# Starts a single-witness private TRON network (java-tron) for the local node tests.
#   gRPC full node: localhost:50051, gRPC solidity: localhost:50061, HTTP: localhost:8090
# Genesis funds THe8MeboLUbvi1TYntzPKK8SsfcCiJCZ19 (a well-known TEST key, never use it elsewhere).
set -euo pipefail
cd "$(dirname "$0")"

IMAGE="${TRON_IMAGE:-tronprotocol/java-tron:latest}"
docker rm -f nexutron-tron-node >/dev/null 2>&1 || true
docker run -d --name nexutron-tron-node --network host \
  -v "$PWD/private.conf:/java-tron/private.conf:ro" \
  "$IMAGE" -c /java-tron/private.conf --witness >/dev/null

echo -n "Waiting for blocks"
for _ in $(seq 1 120); do
  height=$(curl -s --noproxy '*' -m 3 -X POST http://127.0.0.1:8090/wallet/getnowblock \
    | sed -n 's/.*"number": *\([0-9]*\).*/\1/p' || true)
  if [ -n "${height:-}" ] && [ "$height" -gt 1 ]; then echo " ready (height $height)"; exit 0; fi
  echo -n "."; sleep 2
done
echo; docker logs --tail 50 nexutron-tron-node; exit 1
