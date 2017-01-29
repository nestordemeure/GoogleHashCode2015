module GHC.Main

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain
open GHC.Import
open GHC.Solve
open GHC.Export
open System.Collections.Generic

//-------------------------------------------------------------------------------------------------
// EVALUATION

let evaluation poolNum rowNum (rows : Row array) =
    let capa = Array.init poolNum (fun _ -> Array.create rowNum 0)

    for r = 0 to rowNum - 1 do 
        let row = rows.[r]
        for slot in row do 
            for serveur in slot.serveurs do 
                capa.[serveur.pool].[r] <- capa.[serveur.pool].[r] + serveur.capa

    let mutable garan = System.Int32.MaxValue
    for p = 0 to poolNum - 1 do 
        let poolCapa = (Array.sum capa.[p]) - (Array.max capa.[p])
        garan <- min garan poolCapa

    garan

//-------------------------------------------------------------------------------------------------
// MAIN

[<EntryPoint>]
let main argv =
    //printfn "%A" argv
    // import
    let inPath = "../dc.in"
    let rows,serveurs,poolNum = import inPath
    // solution
    let newRows = solution rows serveurs poolNum
    // evaluation
    let score = evaluation poolNum rows.Length newRows
    printfn "score : %d" score
    //export 
    //export "../output.txt" (Array.ofList sol.rows)
    0 // return an integer exit code
