module GHC.Export

open ExtCore.Collections
open System.IO

open GHC.Extensions
open GHC.Domain

// 14h - 18h

//-------------------------------------------------------------------------------------------------

/// turns a list of strings into a single string (containing the elements IN order)
let listToString sep (l : string list) =
    match l with 
    | [] -> ""
    | _ -> List.reduce (fun acc s -> acc + sep + s ) l

//-------------------------------------------------------------------------------------------------
// EXPORTATION

let export path lines =
   //File.WriteAllText(path, text)
   File.WriteAllLines(path, lines)
