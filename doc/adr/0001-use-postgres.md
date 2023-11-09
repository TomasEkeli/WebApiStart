# 1. use-postgres

Date: 2023-11-08

## Status

Status: Accepted on 2023-11-08


## Context

Data-storage needs indicate the need for a database. There are many options, from
relational to document to graph databases. Pricing and availability of hosting
services are also a factor.

## Decision

Postgres is a good choice for a relational database. It is open source, has a
large community and is available as a service from many providers. It is very
capable and can be used for many different use cases. We will use it.

## Consequences

The application needs to connect to a Postgres database. Connection is configured
in the `appsettings.json` file, and can be overridden by environment variables.

The application will attempt to create the database schema on startup, if it does
not exist.
