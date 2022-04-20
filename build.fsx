// Information specific for this solution
let appName = "Weather"
let sln = sprintf "%s.sln" appName // test

// Dependencies and FAKE-specific stuff
// ==============================================
#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip //"
// Use this for IDE support. Not required by FAKE 5.
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
// ==============================================

// We fetch the version from the current buildServer if possible (TeamCity, VSTS etc.)
let version = 
        match BuildServer.buildServer with 
        | TeamFoundation -> BuildServer.buildVersion
        | _ -> "1.0.0-beta4"

// Ensure that the 'build' and 'artifacts' directory exists
let buildDir = __SOURCE_DIRECTORY__ + "/build"
Directory.ensure buildDir
let artifactDir = __SOURCE_DIRECTORY__ + "/artifacts"
Directory.ensure artifactDir

// Clean the 'build' and 'artifacts' folder to prepare for a new build
Target.create "Clean" <| fun _ ->
    Shell.cleanDirs [buildDir; artifactDir;]

// Use the given dotnet SDK to restore packages and build the solution into the 'build' folder
Target.create "Build" <| fun _ ->
    DotNet.publish (fun opt -> { opt with OutputPath = Some <| sprintf "%s/app" buildDir }) sln
//Runtime = Some "win-x64" ;

// Zip the 'build' folder and place the zip in the 'artifacts' folder together with the
// ARM-templates and the upload scripts.
Target.create "Artifact" <| fun _ ->
    let artifactFilename = sprintf "%s/%s.%s.zip" artifactDir appName version

    Shell.copyFile buildDir "deploy.fsx"
    Shell.copyFile buildDir "deploy.fsx.lock"
    Shell.copyFile buildDir ".deployment"

    !! "./build/**/*.*" |> Zip.zip buildDir artifactFilename

    let artifactDirArm = (artifactDir + "/infrastructure/")
    Directory.ensure artifactDirArm
    Shell.copyDir artifactDirArm "infrastructure" (fun f -> not <| f.EndsWith(".azcli"))
    Shell.copyFile artifactDir "provision.fsx"
    Shell.copyFile artifactDir "provision.fsx.lock"

"Clean"
    ==> "Build"
    ==> "Artifact"

Target.runOrDefault "Artifact"
