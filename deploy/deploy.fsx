#r @"tools/FAKE/tools/FakeLib.dll"
#r @"tools/FAKE/tools/Fake.FluentMigrator.dll"

open Fake
open Fake.Azure.Kudu
open Fake.REST

let appName = "MyWebApp.Web"
let connectionStringName = "MyWebAppWeb"

// Access token stuff
let aadInstance = "https://login.microsoftonline.com/"
let tenant = environVarOrFail "AzureAD.Tenant"
let clientId = environVarOrFail "Deploy.Migrator.ClientId"
let certThumbprint = environVarOrFail "Deploy.Migrator.CertThumbprint"
let authority = aadInstance + tenant

// Deployment variables based on the inputs
let hostName = sprintf "%s.Host" appName
let migrationsName = sprintf "%s.Migrations.dll" appName
let kuduConnectionStringName = sprintf "SQLAZURECONNSTR_%s" connectionStringName
let connectionString = environVarOrFail kuduConnectionStringName

Target "MigrateDatabase" <| fun _ ->
    let arguments = sprintf """migrate /connectionString="%s" /clientId="%s" /certThumbPrint="%s" /authority="%s" """ connectionString clientId certThumbprint authority
    let result, messages =
        ExecProcessRedirected
                (fun info ->
                    info.FileName <- sprintf "./%s.Migrations.Runner/%s.Migrations.Runner.exe" appName appName
                    info.Arguments <- arguments
                    info.WorkingDirectory <- __SOURCE_DIRECTORY__)
                (System.TimeSpan.FromMinutes 10.0)

    messages
    |> Seq.iter (fun m -> printfn "%A - %A" m.Timestamp m.Message)

    if not result then
        failwithf "%s.Migrations.Runner.exe returned with a non-zero exit code" appName

Target "KuduStageFiles" <| fun _ ->
    stageFolder hostName (fun _ -> true)

Target "SmokeTests" <| fun _ ->
    let url = sprintf "https://%s.azurewebsites.net/api/health/ping" (environVarOrFail "WEBSITE_SITE_NAME")
    printf ""
    match ExecuteGetCommand "" "" url with
    | response when response.Contains("pong") -> ()
    | _ -> failwith "Endpoint %s did not respond with \"pong\"! Something must be wrong :(" // TODO: Raise alarm even though this should fail the deploy

Target "Deploy" kuduSync

"KuduStageFiles"
    ==> "MigrateDatabase"
    ==> "Deploy"
    ==> "SmokeTests"
    
RunTargetOrDefault "SmokeTests"