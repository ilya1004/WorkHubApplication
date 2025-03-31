#!/bin/bash
set -e

echo "Configuring PostgreSQL Primary..."

echo "wal_level = replica" >> /var/lib/postgresql/data/postgresql.conf
echo "max_wal_senders = 10" >> /var/lib/postgresql/data/postgresql.conf
echo "wal_keep_size = 512MB" >> /var/lib/postgresql/data/postgresql.conf

echo "host replication replication_user 0.0.0.0/0 md5" >> /var/lib/postgresql/data/pg_hba.conf

psql -U postgres -c "CREATE ROLE replication_user WITH REPLICATION LOGIN ENCRYPTED PASSWORD 'replpass';"

echo "Primary DB configured successfully!"
