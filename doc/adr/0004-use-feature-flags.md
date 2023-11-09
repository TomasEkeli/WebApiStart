# 4. use-feature-flags

Date: 2023-11-09

## Status

Status: Accepted on 2023-11-09
Amends [0003-continuous-delivery-through-github-actions.md](0003-continuous-delivery-through-github-actions.md) on 2023-11-09

## Context

Continuous delivery means that code will go straight out to the deployed application. Instead of protecting the running application with long-living branches that merge into the main-branch at irregular intervals, we will protect the application with feature flags.

## Decision

Logical features and paths through the application that are not ready for activation will be disabled with [feature flags](https://en.wikipedia.org/wiki/Feature_toggle). Feature-flags will use the [Feature Management](https://github.com/microsoft/FeatureManagement-Dotnet) functionality in dotnet, and store the state of the features in `appsettings.json`.

To clean up when the feature is complete and delivered we will remove the feature-flag and the checks for it.

## Consequences

We will have more conditionals in our code, and must keep the solution in a working state while making new functionality. We must also manage when features are activated and removed.