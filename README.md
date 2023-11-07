# WebApi starter template

Starting up a new web-api -project with dotnet I find myself repeating a lot of
the same things.

This is a repository with the basic setup:
- devcontainer to develop c# 7 with a postgres database
- a sln -file (proj.sln, should be renamed when using this as a template)
- a web-api project (src/Backend, could be renamed when using this as a template)
- a test project (test/Backend.Tests, could be renamed when using this as a template)
- basic setup for creating and using the database in Backend through Dapper
- basic tests for the Backend using the api exercising:
  - OpenApi (/swagger)
  - healthcheck (/healthz)

## License

GPLv3 - see LICENSE file. This is a template repository, if the license does not work for you - talk to me or create your own repository from scratch.

## Publish command

```bash
cd src/Backend
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer -c Release
```

## Versioning

[NerdBank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) has been added to the project. This means that the version is automatically updated based on the git history. Control the main numbers of the version in the file `version.json`, in the root of the repository.

## TODO
- [x] ~~Dockerfile~~ Set up docker publish for the Backend
- [x] Github action
- [x] OpenTelemetry setup
- [ ] Documentation -templates
- [x] Architectural decision records -templates