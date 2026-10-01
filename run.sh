#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Activate venv if it exists, otherwise warn user
if [ -d ".venv" ]; then
    source .venv/bin/activate
fi

show_usage() {
    echo "Usage: $0 {app|test|install}"
    echo ""
    echo "Commands:"
    echo "  app      - Run the contest app"
    echo "  test     - Run the test suite"
    echo "  install  - Install dependencies"
}

install_deps() {
    echo "Installing dependencies..."
    pip install -r requirements.txt
    cd contest_app
    pip install -e .
    cd ..
}

run_app() {
    echo "Starting contest app..."
    python -m rats.main_window
}

run_tests() {
    echo "Running tests..."
    cd contest_app
    pytest tests/ -v
}

if [ $# -eq 0 ]; then
    show_usage
    exit 1
fi

case "$1" in
    app)
        run_app
        ;;
    test)
        run_tests
        ;;
    install)
        install_deps
        ;;
    *)
        echo "Unknown command: $1"
        show_usage
        exit 1
        ;;
esac
