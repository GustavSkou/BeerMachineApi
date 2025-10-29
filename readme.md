generate certificates
dotnet dev-certs https --trust

easier to test
dotnet run --launch-profile http                <- run program with launch profile http, which exposes the http endpoint
https://localhost:5107/beerMachine/status       <- status endpoint
                                                

dotnet run --launch-profile https               <- for the https
https://localhost:7040/beerMachine/status


for testing post use /BeerMachineApi.http and launch http profile