.PHONY: install dev devw server server-windows web

APP_DIR := app
SERVER_DIR := $(APP_DIR)/server
WEB_DIR := $(APP_DIR)/web

install:
	bash ./scripts/install.sh

dev:
	$(MAKE) -j2 server web

devw:
	$(MAKE) -j2 server-windows web

server-windows:
	cd $(SERVER_DIR) && .venv/Scripts/activate uvicorn.exe api.main:app --reload

server:
	cd $(SERVER_DIR) && source venv/bin/activate && uvicorn api.main:app --reload

web:
	cd $(WEB_DIR) && npm run dev
