#!/bin/bash

echo "Kafka is up. Creating topics..."

KAFKA_BROKER="kafka:9092"

TOPICS=(
    "employer_account_id_saving"
    "freelancer_account_id_saving"
    "payment_intent_saving"
    "payment_cancellation"
)

for TOPIC in "${TOPICS[@]}"; do
    kafka-topics --create --if-not-exists --bootstrap-server $KAFKA_BROKER --replication-factor 1 --partitions 1 --topic $TOPIC
done

echo "All topics created."
