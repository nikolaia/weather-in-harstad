let appName = "Weather"
let sln = sprintf "%s.sln" appName

#r "paket:nuget Fake.Core.Target //"
#r "paket:nuget Fake.DotNet.Cli //"
#r "paket:nuget Fake.IO.FileSystem //"
#r "paket:nuget Fake.IO.Zip //"

// ==============================================
// Use this for IDE support. Not required by FAKE 5. Change "build.fsx" to the name of your script.
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators

let version = 
        match BuildServer.buildServer with 
        | TeamFoundation -> BuildServer.buildVersion
        | _ -> "1.0.0-beta4"

let buildDir = __SOURCE_DIRECTORY__ + "/build"
Directory.ensure buildDir
let artifactDir = __SOURCE_DIRECTORY__ + "/artifacts"
Directory.ensure artifactDir
Target.create "Clean" <| fun _ ->
    Shell.cleanDirs [buildDir; artifactDir;]

Target.create "Build" <| fun _ ->
    let install = lazy DotNet.install DotNet.Release_2_1_4
    let inline dotnetSimple arg = DotNet.Options.lift install.Value arg 
    DotNet.build (fun opt -> { opt with OutputPath = Some buildDir } |> dotnetSimple) "Weather.sln" 
    
Target.create "Artifact" <| fun _ ->
    let artifactFilename = sprintf "%s/%s.%s.zip" artifactDir appName version

    !! "./build/**/*.*"
    |> Zip.zip buildDir artifactFilename

    let artifactDirArm = (artifactDir + "/arm/")
    Directory.ensure artifactDirArm
    Shell.copyDir artifactDirArm "infrastructure" (fun _ -> true)

    Shell.copyFile artifactDir "upload.cmd"
    Shell.copyFile artifactDir "upload.ps1"

open Fake.Core.TargetOperators

"Clean"
    ==> "Build"
    ==> "Artifact"

Target.runOrDefault "Build"