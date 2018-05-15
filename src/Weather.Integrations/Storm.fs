namespace Weather.Integrations

module Storm =

    open FSharp.Data
    open Weather.Models.Domain

    [<Literal>]
    let keySample = "{ \"Key\": \"/C+gqzCFnaDIuVdOktrUwmQql4U0VEQUu80hstkqNy8=\" }"
        
    type StormKey = JsonProvider<keySample>
    type StormPlaces = JsonProvider<"./samples/storm_places.json">
    type StormForecast = JsonProvider<"./samples/storm_now-forecast.json">

    let authUrl = "http://webapi.stormgeo.com/api/v1/keygen/k05ap8ng3k2" // key from github issue somewhere

    let GetForecast locationQuery =
        async {
        
            let! authKey = 
                StormKey.AsyncLoad(authUrl)
            
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