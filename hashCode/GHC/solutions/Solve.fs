module GHC.Solve

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

type State = {fullSize : int ; garantedCapa : Capa ; rows : Row list}

//-------------------------------------------------------------------------------------------------

let fitInSlot serveur slot = if serveur.size > slot.length then System.Int32.MaxValue else slot.length - serveur.size

let fitInRow serveur (row : Row) = 
      let bestSlot = List.minBy (fitInSlot serveur) row
      fitInSlot serveur bestSlot

let insertServerInRow serveur row =
      let bestSlot = List.minBy (fitInSlot serveur) row
      let rec insert row = 
            match row with 
            | [] -> []
            | slot::q when slot <> bestSlot -> 
                  slot::(insert q)
            | slot::q ->
                  {slot with length = slot.length - serveur.size ; serveurs = serveur::slot.serveurs}::q
      insert row

let insertServer serveur rows =
      let bestRow = Array.minBy (fitInRow serveur) rows 
      if fitInRow serveur bestRow <> System.Int32.MaxValue then 
            let newBestRow = insertServerInRow serveur bestRow
            let i = Array.findIndex (fun r -> r = bestRow) rows
            rows.[i] <- newBestRow

//-------------------------------------------------------------------------------------------------
// SOLUTION

/// solution
let solution (rows : Row array) (serveurs:Server array) poolNum =
      let rng = System.Random()
      let serveurs =
            serveurs
            |> Array.sortByDescending (fun se -> se.size)
            |> Array.map (fun se -> {se with pool = rng.Next(poolNum) })
      
      for serveur in serveurs do 
            insertServer serveur rows
      rows



