namespace Weather.Models

module Domain =
    type TemperatureProvider =
        | Yr
        | Storm
        | Sql
        member this.ProviderString =
            match this with
            | Yr -> "Yr"
            | Storm -> "Storm"
            | Sql -> "Sql"

    type Temperature =
        { Provider: TemperatureProvider
          Temperature: decimal option }
        member this.TempString =
            match this.Temperature with
            | Some t -> sprintf "%G°" t
            | None -> "-"
