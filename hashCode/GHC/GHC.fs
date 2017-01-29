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

let mutable score = 0

let evaluation solution = ()

//-------------------------------------------------------------------------------------------------
// MAIN

[<EntryPoint>]
let main argv =
    //printfn "%A" argv
    // import
    let inPath = "../dc.in"
    let rows,serveurs,poolNum = import inPath
    // solution
    let sol = solution rows serveurs poolNum
    // evaluation
    //evaluation sol
    printfn "score : %d" sol.garantedCapa
    //export 
    //export "../output.txt" (Array.ofList sol.rows)
    0 // return an integer exit code
