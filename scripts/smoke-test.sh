#!/usr/bin/env bash
set -euo pipefail

FRONTEND_URL="${FRONTEND_URL:-http://localhost:4173}"
API_URL="${API_URL:-http://localhost:5154}"
TEMP_DIR="$(mktemp -d)"
trap 'rm -rf "$TEMP_DIR"' EXIT

assert_status() {
    local label="$1"
    local url="$2"
    local expected="$3"
    local actual

    actual="$(curl -sS -o "$TEMP_DIR/response" -w '%{http_code}' "$url")"
    if [[ "$actual" != "$expected" ]]; then
        printf 'FAIL %s: esperado HTTP %s, recibido %s (%s)\n' "$label" "$expected" "$actual" "$url" >&2
        exit 1
    fi

    printf 'PASS %s: HTTP %s\n' "$label" "$actual"
}

assert_status 'Frontend home' "$FRONTEND_URL/" 200

assert_status 'OpenAPI document' "$API_URL/swagger/v1/swagger.json" 200
curl -fsS "$API_URL/swagger/v1/swagger.json" -o "$TEMP_DIR/openapi.json"
node -e '
const document = require(process.argv[1]);
if (!document.paths["/api/CULTURA"]) {
  console.error("FAIL OpenAPI contract: falta /api/CULTURA");
  process.exit(1);
}
console.log(`PASS OpenAPI contract: ${Object.keys(document.paths).length} rutas, incluye /api/CULTURA`);
' "$TEMP_DIR/openapi.json"

assert_status 'Weather API' "$API_URL/WeatherForecast" 200
curl -fsS "$API_URL/WeatherForecast" -o "$TEMP_DIR/weather.json"
node -e '
const forecasts = require(process.argv[1]);
if (!Array.isArray(forecasts) || forecasts.length !== 5) {
  console.error("FAIL Weather API contract: se esperaba una lista de cinco elementos");
  process.exit(1);
}
console.log(`PASS Weather API contract: ${forecasts.length} elementos`);
' "$TEMP_DIR/weather.json"

assert_status 'Protected API without token' "$API_URL/api/CULTURA" 401
assert_status 'Vite API proxy without token' "$FRONTEND_URL/api/CULTURA" 401