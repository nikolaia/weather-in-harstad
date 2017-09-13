#I "../../../../.nuget/packages/FSharp.Data/2.3.3/lib/net40/"
#r "FSharp.Data.dll"

open FSharp.Data

[<Literal>]
let keySample = "{ \"Key\": \"/C+gqzCFnaDIuVdOktrUwmQql4U0VEQUu80hstkqNy8=\" }"

type StormKey = JsonProvider<keySample>
type StormPlaces = JsonProvider<"./samples/storm_places.json">
type StormForecast = JsonProvider<"./samples/storm_now-forecast.json">

let authUrl = "http://webapi.stormgeo.com/api/v1/keygen/k05ap8ng3k2" // key from github issue somewhere

let locationQuery = "Harstad"

let authKey = 
    StormKey.Load(authUrl)

let urlEncodedKey = authKey.Key |> System.Net.WebUtility.UrlEncode

let url = sprintf "http://webapi.stormgeo.com/api/v1/places/?q=%s&auth=%s" locationQuery urlEncodedKey
    
let location = StormPlaces.Load(url)

let id =
    location.Places
    |> Seq.head
    |> fun place -> place.PlaceId

let url_forecast =
    sprintf "http://webapi.stormgeo.com/api/v1/now-forecast/%i?auth=%s" id urlEncodedKey

let forecast = StormForecast.Load url_forecast

forecast.CurrentForecast
|> Seq.tryHead
|> function
    | Some f -> sprintf "%G" f.Temperature
    | None -> "-"