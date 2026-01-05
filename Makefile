.PHONY: help up down restart logs logs-web ps rebuild-api urls open clean

help:
	@echo "Available targets:"
	@echo "  make up          - Start all services in detached mode and print URLs"
	@echo "  make down        - Stop and remove containers, networks"
	@echo "  make restart     - Restart all running services"
	@echo "  make logs        - Tail logs for all services"
	@echo "  make logs-web    - Tail logs for the web (Angular) service"
	@echo "  make ps          - Show service status"
	@echo "  make rebuild-api - Rebuild only the API image and restart it"
	@echo "  make urls        - Print service URLs"
	@echo "  make open        - Open key URLs in your browser (macOS)"
	@echo "  make clean       - Down and remove volumes (DANGEROUS: wipes DB/uploads)"

up:
	@docker compose up -d --wait
	@$(MAKE) urls

down:
	@docker compose down

restart:
	@docker compose restart

logs:
	@docker compose logs -f --tail=100

logs-web:
	@docker compose logs -f web

ps:
	@docker compose ps

rebuild-api:
	@docker compose build api
	@docker compose up -d api
	@echo "API rebuilt and restarted. See: http://localhost:5099/swagger"

urls:
	@echo "Website: http://localhost:4200"
	@echo "API:     http://localhost:5099  (Swagger: http://localhost:5099/swagger)"
	@echo "Uploads: http://localhost:3001"
	@echo "pgAdmin: http://localhost:5050"

open:
	@if command -v open >/dev/null 2>&1; then \
		open http://localhost:4200 ; \
		open http://localhost:5099/swagger ; \
		open http://localhost:5050 ; \
	else \
		echo "Open these URLs manually:" ; \
		$(MAKE) urls ; \
	fi

clean:
	@docker compose down -v
	@echo "Volumes removed. Database and uploaded files reset."

