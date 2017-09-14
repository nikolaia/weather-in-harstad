namespace Weather.Integrations

module Yr =

    open FSharp.Data
    open System.Xml.Linq
    open Weather.Models.Domain

    type YrForecast = XmlProvider<"./samples/yr_forecast.xml">

    let GetForecastHarstad() =
        async {
            let! harstad = YrForecast.AsyncLoad("http://www.yr.no/place/Norway/Troms/Harstad/Harstad/forecast.xml")
            
            return {
                Provider = Yr
                Temp = harstad.Observations.Weatherstations
                       |> Array.tryHead
                       |> function
                            | Some station -> Some station.Temperature.Value
                            | None -> None }
        } |> Async.StartAsTask