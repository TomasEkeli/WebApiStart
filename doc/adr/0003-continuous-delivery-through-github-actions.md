# 3. continuous-delivery-through-github-actions

Date: 2023-11-08

## Status

Status: Accepted on 2023-11-08
Amended by [0004-use-feature-flags.md](0004-use-feature-flags.md) on 2023-11-09  


## Context

Shortening the time from coding to delivery is important, as it gives us faster feedback on our work. We want to be able to deliver our work as soon as possible, and we want to be able to do it with confidence, and we want to be able to do it in an automated fashion.

We need a [continuous-delivery](https://en.wikipedia.org/wiki/Continous_delivery) deployment pipeline.


## Decision

We will use [Github Actions](https://github.com/features/actions) to build and deploy our application. Whenever we push code to our repository's `main` branch, the application will be built and deployed. We will start with one environment, and may add more later.

The pipeline will
    - build the application as a docker image
    - run automated tests
    - deploy the docker image to a container registry
    - update the deployment in the environment with the new image

## Consequences

Our changes to code will become available in the running application quickly. We will be able to deliver quicker, but will also need to protect the application through comprehensive tests, monitoring, alerting and disabling of features that are not ready for activation.
