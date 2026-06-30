#
# Poor man's integration testing system. It works, though!
# The tests assume that the backend app is already running in
# development mode.
#
# WARNING: running these tests will blow away your development database,
# so that the tests can run with all the tables empty.
# The correct thing to do would probably be to have separate appsettings
# and/or launchsettings, with a "tests" profile, and have the integration
# tests spin up a fresh database, run migrations on it, then spin up
# the backend app pointed at it.
#
# I'm still learning .NET Core, so I'm not familiar with the standard
# way to set up integration tests for it.
# I read through this:
# https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-10.0
# ...but I don't think I have time enough to get into it.
# So, a bash script it is!..
# Other ways to set up quick-n-dirty integration tests would be Postman,
# Bruno, Hurl, etc.
# But one nice thing about bash is we can use `sqlite3` to truncate the
# db tables before running the tests...
#
set -euo pipefail

log() {
    echo "=== $@" >&2
}

requireCmd() {
    command -v "$1" >/dev/null || {
        log "ERROR: missing required command: $1"
        return 1
    }
}

requireCmd jq
requireCmd curl
requireCmd sqlite3

BASE_URL="${BASE_URL:-http://localhost:3001}"

apiGet() {
    curl -s "$BASE_URL/$1"
}

apiPost() {
    curl -s -H Content-Type:application/json "$BASE_URL/$1" -d "$2"
}

assertEqual() {
    test "$1" = "$2" || {
        log "FAIL: $1 != $2"
        return 1
    }
}


###############################################################################
# THE TEST SUITE

log "Clearing the database..."
sqlite3 backend/backend.sqlite 'DELETE FROM TodoItems'

log "Checking for empty array of items..."
assertEqual "$(apiGet items/list | jq -c)" "[]"

log "Adding an item..."
itemId1="$(
    apiPost items/create '{"title": "Test Item 1", "content": "hello"}' \
    | jq .itemId)"
itemId2="$(
    apiPost items/create '{"title": "Test Item 2", "content": "world"}' \
    | jq .itemId)"

log "Checking for array of 2 items..."
assertEqual "$(apiGet items/list | jq length)" 2

log "Checking item details..."
assertEqual "$(apiGet items/details/"$itemId1" | jq -r .title)" "Test Item 1"

log "Updating item..."
assertEqual \
    "$(
        apiPost items/update/"$itemId1" '{"content": "bonjour", "completed": true}' \
        | jq -c '{title, content, completed}')" \
    '{"title":"Test Item 1","content":"bonjour","completed":true}'

log "Test suite OK!"
