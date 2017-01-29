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

let export path poolNum rowNum (rows : Row array) =
    [
        for r = 0 to rowNum - 1 do 
            let row = rows.[r]
            for slot in row do 
                let mutable slotIndex = slot.index
                for serveur in slot.serveurs do 
                    yield (serveur.id, r, slotIndex, serveur.pool)
                    slotIndex <- slotIndex + serveur.size
    ] 
    |> List.sortBy (fun (se,r,s,p) -> se)
    |> List.map (fun (se,r,s,p) -> sprintf "%d %d %d" r s p)
    |> fun lines -> File.WriteAllLines(path, lines)
