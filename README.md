# indy-scca-discord-bot
Discord Bot to help run the Indy SCCA Discord server

## Run With Docker

1. Create a `.env` file in the repo root. You can copy `.env.example` and fill in the real values.
2. Build and start the bot:

```bash
docker compose up -d --build
```

3. Watch logs:

```bash
docker compose logs -f
```

4. Stop the bot:

```bash
docker compose down
```

The container reads these variables from `.env`:

- `MONGODB_URI`
- `DISCORD_TOKEN`
- `MONGODB_DB_NAME`
- `ENVIRONMENT`

The compose file also mounts `./logs` to `/app/logs` so Serilog file output is kept on the host machine.
