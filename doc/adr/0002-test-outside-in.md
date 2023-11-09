# 2. test-outside-in

Date: 2023-11-08

## Status

Status: Accepted on 2023-11-08


## Context

Test-driven development helps us write code that is maintainable and solves the actual needs as we describe them in our tests. We want to use this, but we also want to have confidence that our full application works, not just the separate units.

## Decision

To gain confidence in the application and drive functionality we will test outside-in. We have a test-project that tests the entire solution from the API -level.

## Consequences

We require a running postgres database when running those tests.
