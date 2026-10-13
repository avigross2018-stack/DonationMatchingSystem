# DonationMatchingSystem

## Run the complete system with Docker

Create a root `.env` file with the MySQL and JWT values, then run:

```bash
docker compose up --build
```

The command starts the services in dependency order:

1. MySQL starts and passes its health check.
2. `database-init` creates the database schema and exits successfully.
3. The API starts and verifies that it can connect to MySQL.
4. The frontend starts and proxies `/api` requests to the API.

The frontend is available at `http://localhost:5173`, the API at
`http://localhost:5255`, and Swagger at `http://localhost:5255/swagger`.

To stop the system, run `docker compose down`. Add `--volumes` only when you
also want to delete the persisted MySQL data.
