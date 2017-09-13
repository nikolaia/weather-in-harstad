namespace Weather.Integrations

module Yr =

    open FSharp.Data
    open System.Xml.Linq

    type YrForecast = XmlProvider<"./samples/yr_forecast.xml">

    let GetForecastGvarv() =
        async {
            let! harstad = YrForecast.AsyncLoad("http://www.yr.no/place/Norway/Troms/Harstad/Harstad/forecast.xml")
            
            let temp = 
                harstad.Observations.Weatherstations
                |> Array.tryHead
                |> function
                    | Some station -> sprintf "%G" station.Temperature.Value
                    | None -> "-"
            
            return temp
        } |> Async.StartAsTask