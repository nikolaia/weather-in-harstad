namespace Weather.Integrations

module Sql =

    open Weather.Models.Domain
    open System.Data.SqlClient

    let query conStr =
        seq {
            let con = new SqlConnection(conStr)
            let sqlStr = "SELECT '13.37' AS Temp"
            let cmd = new SqlCommand(sqlStr, con)
            con.Open()
            let reader = cmd.ExecuteReader()
            while reader.Read() do
                yield reader.GetName(0), reader.GetString(0)
            reader.Close()
            con.Close()
        }

    let public GetForecastHarstad (connectionString : string) =

        let temperature = 
            query connectionString 
            |> Map.ofSeq
            |> Map.tryFind "Temp"
            |> Option.map decimal
                        
        { Provider = Sql
          Temperature = temperature }
        