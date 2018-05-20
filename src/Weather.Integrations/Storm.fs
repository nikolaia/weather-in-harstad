namespace Weather.Integrations
open Microsoft.Azure.Services.AppAuthentication
open Microsoft.Azure.KeyVault.WebKey
open Microsoft.Azure.KeyVault

module Storm =

    open FSharp.Data
    open Weather.Models.Domain

    [<Literal>]
    let KeySample = "{ \"Key\": \"/C+gxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx=\" }"
        
    type StormKey = JsonProvider<KeySample>
    type StormPlaces = JsonProvider<"./samples/storm_places.json">
    type StormForecast = JsonProvider<"./samples/storm_now-forecast.json">
    let GetForecast locationQuery stormSecretUri =
        async {
            
            let kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(fun _ resource _ -> 
                let azureServiceTokenProvider = new AzureServiceTokenProvider();
                azureServiceTokenProvider.GetAccessTokenAsync(resource)))
                
            let! authUrlSecret = kv.GetSecretAsync(stormSecretUri) |> Async.AwaitTask

            let! authKey = 
                StormKey.AsyncLoad(authUrlSecret.Value)
            
            let urlEncodedKey = authKey.Key |> System.Net.WebUtility.UrlEncode
            
            let url = sprintf "http://webapi.stormgeo.com/api/v1/places/?q=%s&auth=%s" locationQuery urlEncodedKey
                
            let! location = StormPlaces.AsyncLoad(url)
            
            let id =
                location.Places
                |> Seq.head
                |> fun place -> place.PlaceId
            
            let urlForecast =
                sprintf "http://webapi.stormgeo.com/api/v1/now-forecast/%i?auth=%s" id urlEncodedKey
            
            let! forecast = StormForecast.AsyncLoad urlForecast
            
            return {
                Provider = Storm
                Temp = forecast.CurrentForecast
                       |> Seq.tryHead
                       |> function
                            | Some f -> Some f.Temperature
                            | None -> None }
                
        } |> Async.StartAsTask