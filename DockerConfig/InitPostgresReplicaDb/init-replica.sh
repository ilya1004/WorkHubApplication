#!/bin/bash
set -e

echo "Waiting for primary to be ready..."
until pg_isready -h postgres_primary -U postgres; do sleep 2; done

echo "Configuring PostgreSQL Replica..."
rm -rf /var/lib/postgresql/data/*

su - postgres -c "PGPASSWORD=replpass pg_basebackup -h postgres_primary -U replication_user -D /var/lib/postgresql/data -Fp -Xs -P -R --no-password"

echo "Setting correct permissions..."
chown -R postgres:postgres /var/lib/postgresql/data
chmod 0700 /var/lib/postgresql/data

echo "Replica configured successfully!"

exec su - postgres -c "/usr/lib/postgresql/17/bin/postgres -D /var/lib/postgresql/data"
