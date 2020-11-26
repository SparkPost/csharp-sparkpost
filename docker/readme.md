## Build & Deploy via Docker & Nuget

This docker container will pull the latest version of this library, create a nuget package, and deploy it to Nuget.

The only thing is needs is a valid Nuget API key, from someone that has rights to publish the package to Nuget.

```
cd docker
docker build .  # this will produce a hash key, say 12345678
docker run -e "APIKEY=your_nuget_key_here" 12345678
```

This container has been registered on Docker Hub, and can be run like so:

```
docker run -e "APIKEY=your_nuget_key_here" darrencauthon/csharp-sparkpost
```
