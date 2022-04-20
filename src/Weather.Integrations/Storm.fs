namespace Weather.Integrations

module Storm =

    open System
    open Azure.Identity
    open Serilog
    open Azure.Security.KeyVault.Secrets
    open FSharp.Data
    open Weather.Models.Domain

    [<Literal>]
    let KeySample =
        "{ \"Key\": \"/C+gxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx=\" }"

    type StormKey = JsonProvider<KeySample>
    type StormPlaces = JsonProvider<"./samples/storm_places.json">
    type StormForecast = JsonProvider<"./samples/storm_now-forecast.json">

    let public GetForecast locationQuery (secretClient: SecretClient) (stormSecretName: string) =
        async {
            Log.Information("Fetching {stormSecretName} with managed identity token", stormSecretName)

            let! authUrlSecret =
                secretClient.GetSecretAsync(stormSecretName)
                |> Async.AwaitTask

            let! authKey = StormKey.AsyncLoad(authUrlSecret.Value.Value)

            let urlEncodedKey =
                authKey.Key |> System.Net.WebUtility.UrlEncode

            let url =
                sprintf "http://webapi.stormgeo.com/api/v1/places/?q=%s&auth=%s" locationQuery urlEncodedKey

            let! location = StormPlaces.AsyncLoad(url)

            let id =
                location.Places
                |> Seq.head
                |> fun place -> place.PlaceId

            let urlForecast =
                $"http://webapi.stormgeo.com/api/v1/now-forecast/%i{id}?auth=%s{urlEncodedKey}"

            let! forecast = StormForecast.AsyncLoad urlForecast

            return
                { Provider = Storm
                  Temperature =
                      forecast.CurrentForecast
                      |> Seq.tryHead
                      |> function
                          | Some f -> Some f.Temperature
                          | None -> None }

        }
        |> Async.StartAsTask
