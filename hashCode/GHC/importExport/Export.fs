module GHC.Export

open ExtCore.Collections
open System.IO

open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------
// EXPORTATION

let export path poolNum rowNum serverNum (rows : Row array) =
    let servers = Array.create serverNum None
    for r = 0 to rowNum - 1 do 
        let row = rows.[r]
        for slot in row do 
            let mutable slotIndex = slot.index
            for serveur in slot.serveurs do 
                servers.[serveur.id] <- Some (r, slotIndex, serveur.pool)
                slotIndex <- slotIndex + serveur.size
    servers
    |> Array.map (function None -> "x" | Some (r,s,p) -> sprintf "%d %d %d" r s p)
    |> fun lines -> File.WriteAllLines(path, lines)
